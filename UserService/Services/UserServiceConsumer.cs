using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessageQueue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace UserService.Services
{
    public class UserServiceConsumer : BackgroundService
    {
        private readonly RabbitMQSetting _rabbitMqSetting;
        private IConnection _connection;
        private IChannel _channel;

        private UserService _userUservice;

        public UserServiceConsumer(IOptions<RabbitMQSetting> rabbitMqSetting)
        {
            _rabbitMqSetting = rabbitMqSetting.Value;
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqSetting.HostName,
                UserName = _rabbitMqSetting.UserName,
                Password = _rabbitMqSetting.Password
            };
            _connection = (IConnection?)factory.CreateConnectionAsync().Result;
            _channel = (IChannel?)_connection.CreateChannelAsync().Result;
            _channel.QueueDeclareAsync(queue: _rabbitMqSetting.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            StartConsuming(_rabbitMqSetting.QueueName, stoppingToken);
            await Task.CompletedTask;

        }

        private void StartConsuming(string queueName, CancellationToken cancellationToken)
        {
            _channel.QueueDeclareAsync(queue: _rabbitMqSetting.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                bool processedSuccessfully = false;
                try
                {
                    processedSuccessfully = await ProcessMessageAsync(message);
                }
                catch (Exception ex)
                {
                    //_logger.LogError($"Exception occurred while processing message from queue {queueName}: {ex}");
                }

                if (processedSuccessfully)
                {
                    _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                else
                {
                    _channel.BasicRejectAsync(deliveryTag: ea.DeliveryTag, requeue: true);
                }
            };
            _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

        }

        private async Task<bool> ProcessMessageAsync(string message)
        {
            try
            {
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        }
        public override void Dispose()
        {
            _channel.CloseAsync();
            _connection.CloseAsync();
            base.Dispose();
        }
    }
}

using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueue
{
    public class RabbitMQPublisher<T> : IRabbitMQPublisher<T>
    {
        private readonly RabbitMQSetting _rabbitMqSetting;
        private IConnection connection;
        private IChannel channel;
        public RabbitMQPublisher(IOptions<RabbitMQSetting> rabbitMqSetting)
        {
            _rabbitMqSetting = rabbitMqSetting.Value;
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqSetting.HostName,
                UserName = _rabbitMqSetting.UserName,
                Password = _rabbitMqSetting.Password
            };
            connection = (IConnection?)factory.CreateConnectionAsync().Result;
            channel = (IChannel?)connection.CreateChannelAsync().Result;
            channel.QueueDeclareAsync(queue: _rabbitMqSetting.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }
        public async Task PublishMessageAsync(T message)
        {
            channel.QueueDeclareAsync(queue: _rabbitMqSetting.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            await Task.Run(() => channel.BasicPublishAsync(exchange: "", routingKey: _rabbitMqSetting.QueueName, body: body));
        }
        public async Task PublishMessageAsyncWithQueue(T message, string queueName)
        {
            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);
            channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            await channel.BasicPublishAsync(exchange: "test", routingKey: queueName, body: body);
        }
    }
}

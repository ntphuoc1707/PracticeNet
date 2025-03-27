using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessageQueue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace UserService.Services
{
    public class UserServiceConsumer : BackgroundService
    {
        private readonly RabbitMQSetting _rabbitMqSetting;
        private IConnection _connection;
        private IChannel _channel;

        private UserService _userUservice=new UserService();

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
            _channel.QueueDeclareAsync(queue: _rabbitMqSetting.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            StartConsuming(_rabbitMqSetting.QueueName, stoppingToken);
            await Task.CompletedTask;

        }

        private void StartConsuming(string queueName, CancellationToken cancellationToken)
        {
            _channel.QueueDeclareAsync(queue: _rabbitMqSetting.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var props = ea.BasicProperties;
                var replyProps = new BasicProperties();
                replyProps.CorrelationId = props.CorrelationId;
                if (string.IsNullOrEmpty(replyProps.CorrelationId)) return;
                Hashtable hs=JsonConvert.DeserializeObject<Hashtable>(message);
                try
                {
                    string funcName = hs["Func"].ToString();
                    string inputData = hs["Data"].ToString();
                    Type type = typeof(UserService);
                    MethodInfo method = type.GetMethod(funcName);
                    if (method != null)
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        object result;

                        if (parameters.Length == 0)
                        {
                            result = method.Invoke(null, null);
                        }
                        else
                        {
                            Type paramType = parameters[0].ParameterType;
                            object paramObject = JsonConvert.DeserializeObject(inputData, paramType);
                            result = method.Invoke(_userUservice, new object[] { paramObject });
                            var responseBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
                            await _channel.BasicPublishAsync(exchange: "", routingKey: props.ReplyTo, mandatory:false, basicProperties: replyProps, body: responseBytes);
                        }
                        _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        _channel.BasicRejectAsync(deliveryTag: ea.DeliveryTag, requeue: true);
                    }
                }
                catch (Exception ex)
                {
                    _channel.BasicRejectAsync(deliveryTag: ea.DeliveryTag, requeue: true);
                }
            };
            _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

        }

        private async Task<bool> ProcessMessageAsync(Hashtable hs)
        {
            try
            {
                string funcName = hs["Func"].ToString();
                string inputData = hs["Data"].ToString();
                Type type = typeof(UserService);
                MethodInfo method = type.GetMethod(funcName);
                if (method != null)
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    object result;

                    if (parameters.Length == 0)
                    {
                        result = method.Invoke(null, null);
                    }
                    else
                    {
                        Type paramType = parameters[0].ParameterType;
                        object paramObject = JsonConvert.DeserializeObject(inputData, paramType);
                        result = method.Invoke(null, new object[] { paramObject });
                    }
                    return true;
                }
                else
                {
                    return false;
                }
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

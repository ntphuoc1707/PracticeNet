using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MessageQueue
{
    public class RabbitMQPublisher<T> : IRabbitMQPublisher<T>
    {
        private readonly RabbitMQSetting _rabbitMqSetting;
        private IConnection connection;
        private IChannel channel;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _pendingResponses;

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

            _pendingResponses = new ConcurrentDictionary<string, TaskCompletionSource<string>>();

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var correlationId = ea.BasicProperties.CorrelationId;
                if (_pendingResponses.TryRemove(correlationId, out var tcs))
                {
                    var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                    tcs.SetResult(response);
                }
            };
            channel.BasicConsumeAsync(_rabbitMqSetting.QueueName, true, consumer);

        }
        public async Task PublishMessageAsync(T message)
        {
            channel.QueueDeclareAsync(queue: _rabbitMqSetting.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            await Task.Run(() => channel.BasicPublishAsync(exchange: "", routingKey: _rabbitMqSetting.QueueName, body: body));
        }
        public async Task<string> PublishMessageAsyncWithQueue(T message, string queueName, string funcName)
        {
            Hashtable hs = new Hashtable();
            hs.Add("Func", funcName);
            hs.Add("Data", message);
            var messageJson = JsonConvert.SerializeObject(hs);
            var body = Encoding.UTF8.GetBytes(messageJson);
            channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var props = new BasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.ReplyTo = _rabbitMqSetting.QueueName;
            props.CorrelationId = correlationId;

            var tcs = new TaskCompletionSource<string>();
            _pendingResponses[correlationId] = tcs;

            await channel.BasicPublishAsync(exchange: "", routingKey: queueName, mandatory: false, basicProperties: props, body: body);
            return await tcs.Task;
        }
    }
}

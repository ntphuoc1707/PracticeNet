using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueue
{
    public interface IRabbitMQPublisher<T>
    {
        Task PublishMessageAsync(T message);
        Task<string> PublishMessageAsyncWithQueue(T message, string queueName, string funcName);

    }
}

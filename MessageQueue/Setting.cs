namespace MessageQueue
{
    public class RabbitMQSetting
    {
        public string? HostName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? QueueName { get; set; }
    }

    //RabbitMQ Queue name
    public static class RabbitMQQueues
    {
        public const string UserServiceQueue = "UserServiceQueue";
        public const string AuthenticationServiceQueue = "AuthenticationServiceQueue";
    }
}

namespace OrderApi.Services
{
    public interface IRabbitMQPublisher
    {
        void Publish(string queueName, string message);
    }
}

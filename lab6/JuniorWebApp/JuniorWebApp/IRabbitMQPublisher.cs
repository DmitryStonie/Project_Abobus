namespace JuniorsWebApp;

public interface IRabbitMQPublisher
{
    Task PublishMessageAsync(byte[] message, string queueName);
}
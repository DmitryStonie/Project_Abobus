using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace JuniorsWebApp;

public class RabbitMQPublisher : IRabbitMQPublisher
{
    private IConfiguration _configuration;


    public RabbitMQPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task PublishMessageAsync(byte[] message, string queueName)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        
        using var connection = factory.CreateConnection();
        using var channelOut = connection.CreateModel();
        channelOut.ExchangeDeclare(exchange: "wishlists", type: ExchangeType.Fanout);

        var messageJson = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(messageJson);

        await Task.Run(() =>
        {
            channelOut.BasicPublish(exchange: "wishlists", 
                routingKey: string.Empty,
                basicProperties: null,
                body: body); 
        });
    }
}
using System.Text;
using Hackathon;
using Hackathon.DataProviders;
using Hackathon.Migrations;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace JuniorsWebApp;

public class JuniorService(
    IHostApplicationLifetime appLifetime,
    IConfiguration configuration,
    IDataLoadingInterface dataLoader,
    IWishListGenerator wishlistGenerator,
    ILogger<JuniorService> logger) : IHostedService
{
    private bool _running = true;
    private List<TeamLead> _teamLeads;
    private Employee _junior;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(RunAsync);
        return Task.CompletedTask;
    }

    public void RunAsync()
    {
        logger.LogInformation("Started");
        Init();
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channelIn = connection.CreateModel();
        using var channelOut = connection.CreateModel();
        
        channelIn.ExchangeDeclare(exchange: "hackathons", type: ExchangeType.Fanout);
        channelOut.ExchangeDeclare(exchange: "wishlists", type: ExchangeType.Fanout);
        
        var inQueueName = channelIn.QueueDeclare().QueueName;
        channelIn.QueueBind(queue: inQueueName,
            exchange: "hackathons",
            routingKey: string.Empty);
        
        
        var outQueueName = channelOut.QueueDeclare().QueueName;
        channelOut.QueueBind(queue: outQueueName,
            exchange: "wishlists",
            routingKey: "employees");
        
        
        logger.LogInformation(" [*] Waiting for messages.");
        //get hackathon
        var consumer = new EventingBasicConsumer(channelIn);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            logger.LogInformation($" [x] Received {message}");
            //send wishlist
            // _junior.Wishlist = new Wishlist(wishlistGenerator.CreateWishlist(_teamLeads));
            // body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_junior));
            // channelOut.BasicPublish(exchange: "wishlists", 
            //     routingKey: string.Empty,
            //     basicProperties: null,
            //     body: body);
            // logger.LogInformation($" [x] sended wishlists {message}");
        };
        channelIn.BasicConsume(queue: inQueueName,
            autoAck: true,
            consumer: consumer);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask; // _running = false         

    private void Init()
    {
        try
        {
            //_junior = new Junior(Int32.Parse(configuration["ID"]!), configuration["NAME"]);
            _junior = new Junior(1, "Юдин Адам");
            _teamLeads = dataLoader.LoadTeamLeads();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load team leads");
            throw;
        }
    }
}
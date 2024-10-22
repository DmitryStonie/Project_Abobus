using Hackathon;
using Hackathon.DataProviders;
using MassTransit;
using MassTransitMessages.Messages;

namespace TeamLeadWebApp.Consumers;

public class SubmitHackathonConsumer :
    IConsumer<HackathonStarted>
{
    readonly IBus _bus;
    private readonly IWishListGenerator _generator;
    private int _teamleadId;
    private string _teamleadName;
    private List<Junior> _juniors;

    public SubmitHackathonConsumer()
    {
        
    }
    public SubmitHackathonConsumer(IDataLoadingInterface reader, IBus bus, IWishListGenerator generator, IConfiguration configuration)
    {
        _juniors = reader.LoadJuniors();
        _teamleadId = int.Parse(configuration["ID"]!);
        _teamleadName = configuration["NAME"]!;
        _generator = generator;
        _bus = bus;
    }
    public async Task Consume(ConsumeContext<HackathonStarted> context)
    {
        Console.WriteLine($"Got HackathonStarted id: {context.Message.HackathonId}");
        await context.Publish<TeamleadWishlistSended>(new()
        {
            HackathonId = context.Message.HackathonId,
            Wishlist = _generator.CreateWishlist(_juniors),
            TeamleadId = _teamleadId,
            Name = _teamleadName,
        });
    }
}
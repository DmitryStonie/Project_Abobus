using Hackathon;
using Hackathon.DataProviders;
using MassTransit;

namespace JuniorsWebApp.MassTransit.Consumers;

public class SubmitHackathonConsumer :
    IConsumer<HackathonStarted>
{
    readonly IBus _bus;
    private readonly IWishListGenerator _generator;
    private int _juniorId;
    private string _juniorName;
    private List<TeamLead> _teamLeads;

    public SubmitHackathonConsumer(IDataLoadingInterface reader, IBus bus, IWishListGenerator generator, IConfiguration configuration)
    {
        _teamLeads = reader.LoadTeamLeads();
        _juniorId = int.Parse(configuration["ID"]!);
        _juniorName = configuration["NAME"]!;
        _generator = generator;
        _bus = bus;
    }
    public async Task Consume(ConsumeContext<HackathonStarted> context)
    {
        Console.WriteLine("Got HackathonStarted");
        await context.Publish<JuniorWishlistSended>(new()
        {
            HackathonId = context.Message.HackathonId,
            Wishlist = _generator.CreateWishlist(_teamLeads),
            JuniorId = _juniorId,
            Name = _juniorName,
        });
    }
}
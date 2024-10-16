using Hackathon;
using Hackathon.DataProviders;
using JuniorsWebApp.MassTransit;
using MassTransit;

namespace HRDirectorWebApp.MassTransit.Consumers;

public class GetTeamleadWishlistConsumer :
    IConsumer<TeamleadWishlistSended>
{
    readonly HrManager _hrManager;

    public GetTeamleadWishlistConsumer(HrManager hrManager)
    {
        _hrManager = hrManager;
    }
    public async Task Consume(ConsumeContext<TeamleadWishlistSended> context)
    {
        var teamLead = new TeamLead(0, context.Message.Name, context.Message.TeamleadId, new Wishlist(context.Message.Wishlist));
        _hrManager.TeamLeads.Add(teamLead);
        Console.WriteLine("HRManager got team lead");
    }
}
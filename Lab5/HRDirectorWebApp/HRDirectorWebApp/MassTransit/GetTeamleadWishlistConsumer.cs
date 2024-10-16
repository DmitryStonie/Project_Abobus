using Hackathon;
using JuniorsWebApp.MassTransit;
using MassTransit;

namespace HRDirectorWebApp.MassTransit.Consumers;

public class GetTeamleadWishlistConsumer :
    IConsumer<TeamleadWishlistSended>
{
    readonly HrDirector _hrDirector;

    public GetTeamleadWishlistConsumer(HrDirector hrDirector)
    {
        _hrDirector = hrDirector;
    }
    public async Task Consume(ConsumeContext<TeamleadWishlistSended> context)
    {
        //убрать константу (видимо путем добавления нужного конструктора)
        var teamLead = new TeamLead(0, context.Message.Name, context.Message.TeamleadId, new Wishlist(context.Message.Wishlist));
        _hrDirector.Teamleads.Add(teamLead);
        Console.WriteLine("HRDirector got teamlead");
    }
}   
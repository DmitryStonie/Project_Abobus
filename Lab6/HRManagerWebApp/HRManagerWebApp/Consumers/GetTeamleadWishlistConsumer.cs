using Hackathon;
using HRManagerWebApp.Utilites;
using MassTransit;
using MassTransitMessages.Messages;

namespace HRManagerWebApp.Consumers;

public class GetTeamleadWishlistConsumer :
    IConsumer<TeamleadWishlistSended>
{
    readonly HRManager _hrManager;
    private readonly TeamsSender _teamsSender;
    private readonly IConfiguration _configuration;
    
    public GetTeamleadWishlistConsumer(HRManager hrManager, TeamsSender teamsSender, IConfiguration configuration)
    {
        _hrManager = hrManager;
        _teamsSender = teamsSender;
        _configuration = configuration;
    }
    public async Task Consume(ConsumeContext<TeamleadWishlistSended> context)
    {
        Console.WriteLine($"HRManager got team lead {context.Message.TeamleadId}");
        var teamLead = new TeamLead(context.Message.TeamleadId, context.Message.Name, new Wishlist(context.Message.Wishlist));
        if (_hrManager.AddTeamLead(teamLead))
        {
            if (_hrManager.IsEmployeesEnough() && !_hrManager.IsTriedToSend())
            {
                var teams = _hrManager.GetTeams();
                Console.WriteLine("Try to send");
                await _teamsSender.SendTeams(teams!, _configuration["HR_DIRECTOR_IP"]!, _hrManager.guid);
                Console.WriteLine("Sent");
                _hrManager.Reset();
            }
        }
    }
}
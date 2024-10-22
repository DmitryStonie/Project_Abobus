using Hackathon;
using HRManagerWebApp.Utilites;
using MassTransit;
using MassTransitMessages.Messages;

namespace HRManagerWebApp.Consumers;

public class GetJuniorWishlistConsumer :
    IConsumer<JuniorWishlistSended>
{
    readonly HRManager _hrManager;
    private readonly TeamsSender _teamsSender;
    private readonly IConfiguration _configuration;

    public GetJuniorWishlistConsumer(HRManager hrManager, TeamsSender teamsSender, IConfiguration configuration)
    {
        _hrManager = hrManager;
        _teamsSender = teamsSender;
        _configuration = configuration;
    }
    public async Task Consume(ConsumeContext<JuniorWishlistSended> context)
    {
        Console.WriteLine($"HRManager got junior {context.Message.JuniorId}");
        var junior = new Junior(context.Message.JuniorId, context.Message.Name, new Wishlist(context.Message.Wishlist));
        if (_hrManager.AddJunior(junior))
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
using Hackathon;
using MassTransit;
using MassTransitMessages.Messages;

namespace HRDirectorWebApp.Consumers;

public class GetJuniorWishlistConsumer :
    IConsumer<JuniorWishlistSended>
{
    readonly HRDirector _hrDirector;
    readonly IBus _bus;
    readonly IConfiguration _configuration;
    
    public GetJuniorWishlistConsumer(HRDirector hrDirector, HackathonInviteSender hackathonInviteSender, IBus bus, IConfiguration configuration)
    {
        _hrDirector = hrDirector;
        _bus = bus;
        _configuration = configuration;
    }
    public async Task Consume(ConsumeContext<JuniorWishlistSended> context)
    {
        var junior = new Junior(context.Message.JuniorId, context.Message.Name, new Wishlist(context.Message.Wishlist));
        _hrDirector.AddJunior(junior);
        if (_hrDirector.IsEmployeesEnough() && !_hrDirector.IsTriedToSave())
        {
            _hrDirector.SaveHackathon();
            _hrDirector.Reset();
            if (_hrDirector.GetHoldedHackathons() < Int32.Parse(_configuration["NUMBER_OF_HACKATHONS"]))
            {
                await _bus.Publish(new HackathonStarted() { HackathonId = _hrDirector.GetHackathonId() });
            }

        }
        Console.WriteLine("HRDirector got junior");

    }
}   
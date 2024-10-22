using MassTransit;
using MassTransitMessages.Messages;

namespace HRDirectorWebApp;

public class HackathonInviteSender : BackgroundService
{
    private readonly IBus _bus;
    private readonly int _hackathonId;

    public HackathonInviteSender(IBus bus, HRDirector hrDirector)
    { 
        _bus = bus;
        _hackathonId = hrDirector.GetHackathonId();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _bus.Publish(new HackathonStarted(){HackathonId = _hackathonId}, stoppingToken);
            Console.WriteLine($"HRDirector sended hackathon {_hackathonId}");
        }
    }
}
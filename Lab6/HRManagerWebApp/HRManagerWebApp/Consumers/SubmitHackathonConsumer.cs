using Hackathon;
using Hackathon.DataProviders;
using MassTransit;
using MassTransitMessages.Messages;

namespace HRManagerWebApp.Consumers;

public class SubmitHackathonConsumer :
    IConsumer<HackathonStarted>
{
    private readonly HRManager _hrManager;
    public SubmitHackathonConsumer(HRManager hrManager)
    {
        _hrManager = hrManager;
    }
    public async Task Consume(ConsumeContext<HackathonStarted> context)
    {
        _hrManager.SetHackathonId(context.Message.HackathonId);
        Console.WriteLine($"Got HackathonStarted id: {context.Message.HackathonId}");
    }
}
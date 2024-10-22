using JuniorsWebApp.MassTransit;
using MassTransit;

namespace HRDirectorWebApp;

public class Worker : BackgroundService
{
    readonly IBus _bus;

    public Worker(IBus bus)
    {
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _bus.Publish<HackathonStarted>(new HackathonStarted(), stoppingToken);
            Console.WriteLine($"HRDirector sended hackathonStarted at: {DateTime.Now}");
            await Task.Delay(1000, stoppingToken);
        }
    }
}

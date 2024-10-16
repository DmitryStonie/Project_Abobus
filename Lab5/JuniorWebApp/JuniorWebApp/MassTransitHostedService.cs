using MassTransit;

namespace JuniorsWebApp;

public class MassTransitHostedService : Microsoft.Extensions.Hosting.IHostedService
{
    private IBusControl _busControl;

    public void MassTransitBusControlService(IBusControl busControl)
    {
        this._busControl = busControl;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _busControl.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _busControl.StopAsync(cancellationToken);
    }
}
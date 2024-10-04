using System.Text;
using Hackathon;
using Hackathon.DataProviders;
using Newtonsoft.Json;

namespace TeamLeadWebApp;

public class TeamLeadService(
    IHostApplicationLifetime appLifetime,
    IConfiguration configuration,
    IDataLoadingInterface dataLoader,
    IWishListGenerator wishlistGenerator,
    ILogger<TeamLeadService> logger) : IHostedService
{
    private bool _running = true;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(RunAsync);
        return Task.CompletedTask;
    }

    public void RunAsync()
    {
        var juniors = new List<Junior>();
        var teamLead = new TeamLead();
        try
        {
            teamLead = new TeamLead(Int32.Parse(configuration["ID"]!), configuration["NAME"]);
            juniors = dataLoader.LoadJuniors();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load juniors");
        }

        teamLead.Wishlist = new Wishlist(wishlistGenerator.CreateWishlist(juniors));
        bool wishlistLoaded = false;
        while (_running && !wishlistLoaded)
        {
            using HttpClient client = new HttpClient();
            try
            {
                var json = JsonConvert.SerializeObject(teamLead);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync(configuration["HR_MANAGER_IP"] + "/teamleads", content);
                if (response.Result.IsSuccessStatusCode)
                {
                    logger.LogInformation("Wishlist successfully loaded!");
                    wishlistLoaded = true;
                    appLifetime.StopApplication();
                }
                else
                {
                    logger.LogInformation($"Got status code {response.Result.StatusCode}");
                    wishlistLoaded = true;
                    appLifetime.StopApplication();
                }
            }
            catch (AggregateException ex)
            {
                logger.LogError($"Got connection exception: {ex.Message}");
                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                logger.LogError($"Got fatal exception: {ex.Message}");
                appLifetime.StopApplication();
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask; // _running = false         
}
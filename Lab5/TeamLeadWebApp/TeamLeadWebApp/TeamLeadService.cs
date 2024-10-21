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
    ILogger<TeamLeadService> logger,
    IHttpClientFactory httpClientFactory) : IHostedService
{
    private bool _running = true;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(RunAsync);
        return Task.CompletedTask;
    }

    public async Task RunAsync()
    {
        var juniors = new List<Junior>();
        var teamLead = new TeamLead();
        try
        {
            juniors = dataLoader.LoadJuniors();
            juniors.ForEach(t => t.Id = t.JuniorId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load juniors");
            appLifetime.StopApplication();
        }

        var wishlist = new Wishlist(wishlistGenerator.CreateWishlist(juniors));
        wishlist.InitWishlistById(teamLead.TeamLeadId);
        teamLead = new TeamLead(Int32.Parse(configuration["ID"]!), configuration["NAME"], wishlist);
        bool wishlistLoaded = false;
        logger.LogInformation($"Teamlead {teamLead.JuniorId}Started");
        while (_running && !wishlistLoaded)
        {
            try
            {
                var json = JsonConvert.SerializeObject(teamLead);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClientFactory.CreateClient().PostAsync(configuration["HR_MANAGER_IP"], content);
                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("Wishlist successfully loaded!");
                    wishlistLoaded = true;
                    appLifetime.StopApplication();
                }
                else
                {
                    logger.LogInformation($"Got status code {response.StatusCode}");
                }
            }
            catch (HttpRequestException  ex)
            {
                logger.LogError($"Got connection exception: {ex.Message}");
                await Task.Delay(2000);
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
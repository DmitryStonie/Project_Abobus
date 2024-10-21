using System.Text;
using Hackathon;
using Hackathon.DataProviders;
using Newtonsoft.Json;

namespace JuniorsWebApp;

public class JuniorService(
    IHostApplicationLifetime appLifetime,
    IConfiguration configuration,
    IDataLoadingInterface dataLoader,
    IWishListGenerator wishlistGenerator,
    ILogger<JuniorService> logger,
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
        var teamLeads = new List<TeamLead>();
        var junior = new Junior();
        try
        {
            teamLeads = dataLoader.LoadTeamLeads();
            teamLeads.ForEach(t => t.Id = t.TeamLeadId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load team leads");
            appLifetime.StopApplication();
        }

        var wishlist = new Wishlist(wishlistGenerator.CreateWishlist(teamLeads));
        wishlist.InitWishlistById(junior.JuniorId);
        junior = new Junior(Int32.Parse(configuration["ID"]!), configuration["NAME"], wishlist);
        bool wishlistLoaded = false;
        logger.LogInformation($"Junior {junior.JuniorId}Started");
        while (_running && !wishlistLoaded)
        {
            try
            {
                var json = JsonConvert.SerializeObject(junior);
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
            catch (HttpRequestException ex)
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
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
    ILogger<TeamLeadService> logger, HttpClient httpClient) : IHostedService
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
            juniors = dataLoader.LoadJuniors();
            juniors.ForEach(t => t.Id = t.JuniorId);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load juniors");
        }
        var Wishlist = new Wishlist(wishlistGenerator.CreateWishlist(juniors));
        Wishlist.InitWishlistById(teamLead.TeamLeadId);
        teamLead = new TeamLead(Int32.Parse(configuration["ID"]!), configuration["NAME"], Int32.Parse(configuration["ID"]!), Wishlist);
        foreach (var wish in teamLead.Wishlist.Wishes)
        {
            Console.WriteLine($"{wish.OwnerId}  {wish.PartnerId}  {wish.WishlistId} {wish.Score}");

        }
        bool wishlistLoaded = false;
        while (_running && !wishlistLoaded)
        {
            try
            {
                var json = JsonConvert.SerializeObject(teamLead);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync(configuration["HR_MANAGER_IP"], content);
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
                Task.Delay(2000);
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
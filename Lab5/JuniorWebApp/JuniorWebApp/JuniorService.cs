﻿using System.Text;
using Hackathon;
using Hackathon.DataProviders;
using Newtonsoft.Json;

namespace JuniorsWebApp;

public class JuniorService(
    IHostApplicationLifetime appLifetime,
    IConfiguration configuration,
    IDataLoadingInterface dataLoader,
    IWishListGenerator wishlistGenerator,
    ILogger<JuniorService> logger) : IHostedService
{
    private bool _running = true;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(RunAsync);
        return Task.CompletedTask;
    }

    public void RunAsync()
    {
        var teamLeads = new List<TeamLead>();
        var junior = new Junior();
        try
        {
            logger.LogInformation("Started");
            junior = new Junior(Int32.Parse(configuration["ID"]!), configuration["NAME"]);
            logger.LogInformation("Startedaaa");
            teamLeads = dataLoader.LoadTeamLeads();
            logger.LogInformation("Started");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load team leads");
        }

        junior.Wishlist = new Wishlist(wishlistGenerator.CreateWishlist(teamLeads));
        bool wishlistLoaded = false;
        logger.LogInformation("Started");
        while (_running && !wishlistLoaded)
        {
            using HttpClient client = new HttpClient();
            try
            {
                var json = JsonConvert.SerializeObject(junior);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync(configuration["HR_MANAGER_IP"] + "/juniors", content);
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
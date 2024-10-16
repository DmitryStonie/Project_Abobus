using System.Text;
using Hackathon;
using HRDirectorWebApp.MassTransit.Consumers;
using MassTransit;
using Newtonsoft.Json;

namespace HRManagerWebApp;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Program started");
        CreateHostBuilder(args).Build().Run();
    }
    public static IHostBuilder CreateHostBuilder(params string[] args)
    {
        return Host.CreateDefaultBuilder(args).ConfigureAppConfiguration(configuration =>
        {
            configuration.Sources.Clear();
            configuration.AddJsonFile("appsettings.json", optional: true);
        }).ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        }).ConfigureServices((context, services) =>
        {
            services.AddSingleton<HrManager>(new HrManager(new List<Junior>(), new List<TeamLead>(), new TeamBuildingStrategy()));
            services.AddMassTransit(x =>
            {
                x.AddConsumer<GetJuniorWishlistConsumer>();
                x.AddConsumer<GetTeamleadWishlistConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });

        });
    }
}
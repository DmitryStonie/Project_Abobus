using Hackathon;
using Hackathon.DataProviders;
using System.Text;
using JuniorsWebApp.MassTransit.Consumers;
using MassTransit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace JuniorsWebApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        var hostBuilder = CreateHostBuilder(args).Build();
        hostBuilder.Run();
    }

    public static IHostBuilder CreateHostBuilder(params string[] args)
    {
        return Host.CreateDefaultBuilder(args).ConfigureAppConfiguration(configuration =>
        {
            configuration.Sources.Clear();
            configuration.AddJsonFile("appsettings.json", optional: true);
            configuration.AddEnvironmentVariables();
        }).ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        }).ConfigureServices((hostContext, services) =>
        {
            int juniorId = int.Parse(hostContext.Configuration["ID"]!);
            string juniorName = hostContext.Configuration["NAME"]!;
            services.AddSingleton<IDataLoadingInterface, CsvDataLoader>(service =>
                new CsvDataLoader(hostContext.Configuration));
            services.AddTransient<IWishListGenerator, RandomWishlistGenerator>();
            services.AddSingleton<Junior>(new Junior(juniorId, juniorName));
            services.AddMassTransit(x =>
            {
                x.AddConsumer<SubmitHackathonConsumer>();
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
using Hackathon;
using Hackathon.DataProviders;
using JuniorsWebApp.Consumers;
using MassTransit;

namespace JuniorsWebApp;

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
                configuration.AddEnvironmentVariables();
            }).ConfigureServices((context, services) =>
            {
                services.AddSingleton<IDataLoadingInterface, CsvDataLoader>(service =>
                    new CsvDataLoader(context.Configuration));
                services.AddTransient<IWishListGenerator, RandomWishlistGenerator>();
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<SubmitHackathonConsumer>().Endpoint(e => e.Name = context.Configuration["HACKATHONS_QUEUE_NAME"]);
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
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });
    }
}
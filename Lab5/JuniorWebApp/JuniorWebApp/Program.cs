using Hackathon;
using Hackathon.DataProviders;

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
                services.AddHostedService<JuniorService>();
                services.AddTransient<IDataLoadingInterface, CsvDataLoader>();
                services.AddTransient<IWishListGenerator, RandomWishlistGenerator>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });
    }
}
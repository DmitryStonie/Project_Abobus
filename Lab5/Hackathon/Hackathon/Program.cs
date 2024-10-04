using System;
using Hackathon.Database.SQLite;
using Hackathon.DataProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hackathon
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Program started");
            CreateHostBuilder().Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(params string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureAppConfiguration(configuration =>
                {
                    configuration.Sources.Clear();
                    configuration.AddJsonFile("appsettings.json", optional: true);
                }).ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                    services.AddHostedService<ConsoleApplication>();
                    services.AddTransient<Hackathon>();
                    services.AddTransient<HackathonWorker>();
                    services.AddDbContextFactory<ApplicationContext>(options => options.UseSqlite(connectionString));
                    services.AddTransient<IDataLoadingInterface, SQLiteDataLoader>();
                    services.AddTransient<IDatabaseLoadingInterface, SQLiteDataLoader>();
                    services.AddTransient<IDataSavingInterface, SQLiteDataSaver>();
                    services.AddTransient<IDataInitializationInterface, SQLiteDataInitializator>();
                    services.AddTransient<ITeamBuildingStrategy, TeamBuildingStrategy>();
                    services.AddTransient<IWishListGenerator, RandomWishlistGenerator>();
                })
                .ConfigureLogging(logging => { logging.ClearProviders(); });
        }
    }
}
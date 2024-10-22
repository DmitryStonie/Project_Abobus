using System.Text;
using Hackathon;
using Hackathon.Database.SQLite;
using Hackathon.DataProviders;
using HRDirectorWebApp.MassTransit.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace HRDirectorWebApp;

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
            services.AddHostedService<Worker>();
            services.AddSingleton<HrDirector>(new HrDirector());
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
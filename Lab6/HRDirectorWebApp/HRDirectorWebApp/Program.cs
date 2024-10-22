using Hackathon;
using Hackathon.Database.SQLite;
using Hackathon.DataProviders;
using HRDirectorWebApp.Consumers;
using HRManagerWebApp;
using MassTransit;
using MassTransit.Transports;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HRDirectorWebApp;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Program started");
        var app = BuildApp(args);
        ConfigureRouting(app);
        app.Run();
    }
    public static WebApplication BuildApp(params string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.json");
        builder.Configuration.AddEnvironmentVariables();
        builder.Services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlite(builder.Configuration["DATABASE_CONNECTION_STRING"]),
            ServiceLifetime.Singleton);
        builder.Services.AddSingleton<ReaderWriterLockSlim>();
        builder.Services.AddSingleton<IDataSavingInterface, DataSaver>();
        builder.Services.AddSingleton<JsonBodyReader>();
        builder.Services.AddSingleton<ReadedGuids>();
        builder.Services.AddControllers();
        builder.Services.AddHostedService<HackathonInviteSender>();
        builder.Services.AddTransient<HackathonInviteSender>();
        builder.Services.AddSingleton<HRDirector>();
        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<GetJuniorWishlistConsumer>().Endpoint(e => e.Name = builder.Configuration["JUNIORS_QUEUE_NAME"]);
            x.AddConsumer<GetTeamleadWishlistConsumer>().Endpoint(e => e.Name = builder.Configuration["TEAMLEADS_QUEUE_NAME"]);
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
        return builder.Build();
    }

    public static void ConfigureRouting(WebApplication app)
    {
        app.MapControllerRoute(
            name: "teams",
            pattern: "{controller=Teams}/{action=Post}");
    }
}
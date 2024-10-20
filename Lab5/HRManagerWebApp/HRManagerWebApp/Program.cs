using System.Text;
using Hackathon;
using Hackathon.Database.SQLite;
using Hackathon.DataProviders;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HRManagerWebApp;

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
        builder.Services.AddDbContext<HrManagerApplicationContext>(options =>
                options.UseSqlite(builder.Configuration["DATABASE_CONNECTION_STRING"]),
            ServiceLifetime.Singleton);
        builder.Services.AddTransient<ITeamBuildingStrategy, TeamBuildingStrategy>();
        builder.Services.AddTransient<IDataSavingInterface, HrManagerDataSaver>();
        builder.Services.AddTransient<IDatabaseLoadingInterface, HrManagerDataLoader>();
        builder.Services.AddSingleton<HRManager>();
        builder.Services.AddSingleton<TeamsSender>();
        builder.Services.AddSingleton<JsonBodyReader>();
        builder.Services.AddControllers();
        builder.Services.AddHttpClient();
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        return builder.Build();
    }

    public static void ConfigureRouting(WebApplication app)
    {
        app.MapControllerRoute(
            name: "juniors",
            pattern: "{controller=Juniors}/{action=Post}");
        app.MapControllerRoute(
            name: "teamleads",
            pattern: "{controller=Teamleads}/{action=Post}");
    }
}
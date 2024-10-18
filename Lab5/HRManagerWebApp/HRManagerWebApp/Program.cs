using System.Text;
using Hackathon;
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
        builder.Services.AddSingleton<ITeamBuildingStrategy, TeamBuildingStrategy>();
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
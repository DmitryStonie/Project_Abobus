using Hackathon;
using Hackathon.Database.SQLite;
using Hackathon.DataProviders;
using HRManagerWebApp;
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
        builder.Services.AddTransient<IDataSavingInterface, DataSaver>();
        builder.Services.AddSingleton<JsonBodyReader>();
        builder.Services.AddControllers();
        return builder.Build();
    }

    public static void ConfigureRouting(WebApplication app)
    {
        app.MapControllerRoute(
            name: "teams",
            pattern: "{controller=Teams}/{action=Post}");
    }
}
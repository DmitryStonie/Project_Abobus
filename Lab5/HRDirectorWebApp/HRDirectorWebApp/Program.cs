using Hackathon;
using Hackathon.Database.SQLite;
using Hackathon.DataProviders;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HRDirectorWebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.json");
        builder.Configuration.AddEnvironmentVariables();
        builder.Services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlite(builder.Configuration["DATABASE_CONNECTION_STRING"]),
            ServiceLifetime.Singleton);
        builder.Services.AddTransient<IDataSavingInterface, SQLiteDataSaver>();
        var app = builder.Build();
        app.MapPost("/", async (context) =>
        {
            var bodyStr = await ReadJsonBody(context.Request);
            var teams = JsonConvert.DeserializeObject<List<Team>>(bodyStr);
            app.Logger.LogInformation("Got teams");
            await context.Response.WriteAsync("Ok");
            if (teams != null)
            {
                var hackathon = new Hackathon.Hackathon(teams, app.Services.GetRequiredService<IDataSavingInterface>());
                hackathon.Complete();
                var harmonicMean = hackathon.HarmonicMean;
                Console.WriteLine($"Harmonic mean: {harmonicMean}");
            }
        });
        app.Run();
    }

    private static async Task<String> ReadJsonBody(HttpRequest request)
    {
        request.EnableBuffering();
        request.Body.Position = 0;
        using var stream = new StreamReader(request.Body);
        var bodyStr = await stream.ReadToEndAsync();
        return bodyStr;
    }
}
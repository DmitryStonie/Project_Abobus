using System.Text;
using Hackathon;
using Newtonsoft.Json;

namespace HRManagerWebApp;

public class Program
{
    private static readonly Dictionary<int, Junior> Juniors = new Dictionary<int, Junior>();
    private static readonly Dictionary<int, TeamLead> TeamLeads = new Dictionary<int, TeamLead>();

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.json");
        builder.Configuration.AddEnvironmentVariables();
        builder.Services.AddSingleton<ITeamBuildingStrategy, TeamBuildingStrategy>();
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        var app = builder.Build();
        app.Logger.LogInformation("wait");
        app.MapPost("/juniors", async context =>
        {
            try
            {
                app.Logger.LogInformation("Get junior request");
                var bodyStr = await ReadJsonBody(context.Request);
                var junior = JsonConvert.DeserializeObject<Junior>(bodyStr);
                if (junior != null)
                {
                    context.Response.StatusCode = 200;
                    if (AddJunior(junior))
                    {
                        if (Juniors.Count() + TeamLeads.Count() == Int32.Parse(app.Configuration["EMPLOYEES_COUNT"]!) &&
                            Juniors.Count() == TeamLeads.Count())
                        {
                            var teams = CreateTeams(app.Services.GetService<ITeamBuildingStrategy>()!);
                            SendTeams(teams, app.Configuration["HR_DIRECTOR_IP"]!, app.Logger);
                            CleanEmployees();
                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex.Message);
            }
        });
        app.MapPost("/teamleads", async context =>
        {
            try
            {
                app.Logger.LogInformation("Get teamlead request");
                var bodyStr = await ReadJsonBody(context.Request);
                var teamLead = JsonConvert.DeserializeObject<TeamLead>(bodyStr);
                if (teamLead != null)
                {
                    context.Response.StatusCode = 200;
                    if (AddTeamLead(teamLead))
                    {
                        if ((Juniors.Count() + TeamLeads.Count()) ==
                            Int32.Parse(app.Configuration["EMPLOYEES_COUNT"]!) && Juniors.Count() == TeamLeads.Count())
                        {
                            var teams = CreateTeams(app.Services.GetService<ITeamBuildingStrategy>()!);
                            SendTeams(teams, app.Configuration["HR_DIRECTOR_IP"]!, app.Logger);
                            CleanEmployees();
                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex.Message);
            }
        });
        app.Run();
    }

    private static IEnumerable<Team> CreateTeams(ITeamBuildingStrategy strategy)
    {
        var juniorsWishLists = new List<Wishlist>();
        var teamLeadsWishLists = new List<Wishlist>();
        var juniors = new List<Junior>();
        var teamLeads = new List<TeamLead>();
        foreach (var junior in Juniors.Values)
        {
            juniors.Add(junior);
            juniorsWishLists.Add(junior.Wishlist);
        }

        foreach (var teamLead in TeamLeads.Values)
        {
            teamLeads.Add(teamLead);
            teamLeadsWishLists.Add(teamLead.Wishlist);
        }

        var teams = strategy.BuildTeams(teamLeads, juniors, teamLeadsWishLists, juniorsWishLists);
        return teams;
    }

    private static async Task<String> ReadJsonBody(HttpRequest request)
    {
        request.EnableBuffering();
        request.Body.Position = 0;
        using var stream = new StreamReader(request.Body);
        var bodyStr = await stream.ReadToEndAsync();
        return bodyStr;
    }

    private static bool SendTeams(IEnumerable<Team> teams, string hrDirectorUri,
        ILogger appLogger)
    {
        while (true)
        {
            try
            {
                using HttpClient client = new HttpClient();
                var json = JsonConvert.SerializeObject(teams);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync(hrDirectorUri, content);
                if (response.Result.IsSuccessStatusCode)
                {
                    appLogger.LogInformation("Teams successfully loaded!");
                    return true;
                }

                appLogger.LogInformation($"Got response {response.Result.StatusCode}");
            }
            catch (AggregateException ex)
            {
                appLogger.LogError(ex.Message);
            }
            catch (Exception ex)
            {
                appLogger.LogError(ex.Message);
                return false;
            }
        }
    }

    private static void CleanEmployees()
    {
        Juniors.Clear();
        TeamLeads.Clear();
    }

    private static bool AddJunior(Junior junior)
    {
        if (Juniors.ContainsKey(junior.JuniorId))
        {
            return false;
        }

        junior.Wishlist.InitWishlist();
        Juniors[junior.JuniorId] = junior;
        return true;
    }

    private static bool AddTeamLead(TeamLead teamLead)
    {
        if (TeamLeads.ContainsKey(teamLead.TeamLeadId))
        {
            return false;
        }

        teamLead.Wishlist.InitWishlist();
        TeamLeads[teamLead.TeamLeadId] = teamLead;
        return true;
    }
}
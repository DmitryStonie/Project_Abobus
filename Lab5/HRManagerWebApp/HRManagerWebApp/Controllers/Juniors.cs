using Hackathon;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HRManagerWebApp.Controllers;


public class Juniors(ILogger<Juniors> logger, IConfiguration configuration, HRManager hrManager, TeamsSender teamsSender, JsonBodyReader reader) : Controller
{
    public async Task Post()
    {
        try
        {
            logger.LogInformation("Get junior request");
            var bodyStr = await reader.ReadJsonBody(Request);
            var junior = JsonConvert.DeserializeObject<Junior>(bodyStr);
            if (junior != null)
            {
                Response.StatusCode = 200;
                if (hrManager.AddJunior(junior))
                {
                    if (hrManager.GetJuniorsCount() + hrManager.GetTeamleadsCount() == Int32.Parse(configuration["EMPLOYEES_COUNT"]!) &&
                        hrManager.GetJuniorsCount() == hrManager.GetTeamleadsCount())
                    {
                        var teams = hrManager.CreateTeams();
                        await teamsSender.SendTeams(teams, configuration["HR_DIRECTOR_IP"]!);
                        hrManager.ClearEmployees();
                    }
                }
            }
            else
            {
                Response.StatusCode = 400;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
        }
    }
}
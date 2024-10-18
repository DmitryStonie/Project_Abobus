using Hackathon;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HRManagerWebApp.Controllers;

public class Teamleads(
    ILogger<TeamLead> logger,
    IConfiguration configuration,
    HRManager hrManager,
    TeamsSender teamsSender,
    JsonBodyReader reader) : Controller
{
    public async Task Post()
    {
        try
        {
            logger.LogInformation("Get teamlead request");
            var bodyStr = await reader.ReadJsonBody(Request);
            var teamLead = JsonConvert.DeserializeObject<TeamLead>(bodyStr);
            if (teamLead != null)
            {
                Response.StatusCode = 200;
                if (hrManager.AddTeamLead(teamLead))
                {
                    logger.LogInformation(teamLead.ToString());
                    if (hrManager.GetJuniorsCount() + hrManager.GetTeamleadsCount() ==
                        Int32.Parse(configuration["EMPLOYEES_COUNT"]!) &&
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
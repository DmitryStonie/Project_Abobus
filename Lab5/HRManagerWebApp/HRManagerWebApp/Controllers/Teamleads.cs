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
                if (hrManager.AddTeamLead(teamLead))
                {
                    if (hrManager.IsEmployeesEnough()) 
                    {
                        logger.LogInformation($"try to make teams and send tl");
                        var teams = hrManager.GetTeams();
                        foreach (var team in teams)
                        {
                            logger.LogInformation($"{team.Junior}  {team.TeamLead}");
                        }
                        await teamsSender.SendTeams(teams, configuration["HR_DIRECTOR_IP"]!, hrManager.guid);
                        hrManager.Reset();
                    }
                }
                Response.StatusCode = 200;
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
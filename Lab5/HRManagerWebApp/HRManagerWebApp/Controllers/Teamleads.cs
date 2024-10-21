using Hackathon;
using HRManagerWebApp.Utilites;
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
    public async Task<ActionResult> Post()
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
                        if (hrManager.IsEmployeesEnough() && !hrManager.IsTriedToSend())
                        {
                            var teams = hrManager.GetTeams();
                            Console.WriteLine("Try to send");
                            await teamsSender.SendTeams(teams!, configuration["HR_DIRECTOR_IP"]!, hrManager.guid);
                            Console.WriteLine("sended");
                            hrManager.Reset();
                        }
                }
                return Ok();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
        }
        return BadRequest();
    }
}
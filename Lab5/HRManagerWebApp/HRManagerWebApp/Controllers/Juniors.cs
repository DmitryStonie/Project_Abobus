using Hackathon;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HRManagerWebApp.Controllers;

public class Juniors(
    ILogger<Juniors> logger,
    IConfiguration configuration,
    HRManager hrManager,
    TeamsSender teamsSender,
    JsonBodyReader reader) : Controller
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
                if (hrManager.AddJunior(junior))
                {
                    if (hrManager.IsEmployeesEnough())
                    {
                        var teams = hrManager.GetTeams();
                        await teamsSender.SendTeams(teams!, configuration["HR_DIRECTOR_IP"]!, hrManager.guid);
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
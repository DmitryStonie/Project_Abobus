using Hackathon;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HRManagerWebApp.Controllers;

public class Juniors(
    ILogger<Juniors> logger,
    IConfiguration configuration,
    HRManager hrManager,
    TeamsSender teamsSender,
    JsonBodyReader reader, IHttpClientFactory httpClientFactory) : Controller
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
                await Response.WriteAsync("Ok");
            }
            else
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("Bad request");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
        }
    }
}
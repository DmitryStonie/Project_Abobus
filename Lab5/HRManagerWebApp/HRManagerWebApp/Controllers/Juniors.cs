using Hackathon;
using HRManagerWebApp.Utilites;
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
    public async Task<ActionResult> Post()
    {
        try
        {
            logger.LogInformation("Get junior request");
            var bodyStr = await reader.ReadJsonBody(Request);
            var junior = JsonConvert.DeserializeObject<Junior>(bodyStr);
            Console.WriteLine(Request.Host);
            if (junior != null)
            {
                if (hrManager.AddJunior(junior))
                {
                        if (hrManager.IsEmployeesEnough() && !hrManager.IsTriedToSend())
                        {
                            var teams = hrManager.GetTeams();
                            Console.WriteLine("Try to send");
                            await teamsSender.SendTeams(teams!, configuration["HR_DIRECTOR_IP"]!, hrManager.guid);
                            Console.WriteLine("Sent");
                            hrManager.Reset();
                        }
                }
                Console.WriteLine("Sends ok");
                return Ok();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
        }
        Console.WriteLine("Sends bad request");
        return BadRequest();
    }
}
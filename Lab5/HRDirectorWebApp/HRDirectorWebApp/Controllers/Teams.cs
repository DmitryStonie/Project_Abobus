using System.Diagnostics;
using Hackathon;
using Hackathon.DataProviders;
using HRManagerWebApp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HRDirectorWebApp.Controllers;

public class Teams(ILogger<Teams> logger, IDataSavingInterface dataSaver, JsonBodyReader reader) : Controller
{
    public async Task Post()
    {
        var bodyStr = await reader.ReadJsonBody(Request);
        var teams = JsonConvert.DeserializeObject<List<Team>>(bodyStr);
        logger.LogInformation("Got teams");
        await Response.WriteAsync("Ok");
        if (teams != null)
        {
            foreach (var team in teams)
            {
                logger.LogInformation("Teamlead: " + team.TeamLead.ToString());
                logger.LogInformation("Junior: " + team.Junior.ToString());
            }
            var hackathon = new Hackathon.Hackathon(teams, dataSaver);
            hackathon.Complete();
            var harmonicMean = hackathon.HarmonicMean;
            Console.WriteLine($"Harmonic mean: {harmonicMean}");
        }
    }
}
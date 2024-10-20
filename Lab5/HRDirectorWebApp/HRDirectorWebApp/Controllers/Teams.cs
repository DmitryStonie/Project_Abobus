using System.Diagnostics;
using Hackathon;
using Hackathon.DataProviders;
using HRManagerWebApp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HRDirectorWebApp.Controllers;

public class Teams(ILogger<Teams> logger, IDataSavingInterface dataSaver, JsonBodyReader reader, ReaderWriterLockSlim readWriteLock, ReadedGuids readedGuids) : Controller
{
    public async Task Post()
    {
        var bodyStr = await reader.ReadJsonBody(Request);
        var hackathonTeams = JsonConvert.DeserializeObject<HackathonTeams>(bodyStr);
        var teams = hackathonTeams.teams;
        var guid = hackathonTeams.guid;
        logger.LogInformation("Got teams");
        await Response.WriteAsync("Ok");
        if (teams != null)
        {
            foreach (var team in teams)
            {
                logger.LogInformation("Teamlead: " + team.TeamLead.ToString());
                logger.LogInformation("Junior: " + team.Junior.ToString());
            }
            readWriteLock.EnterWriteLock();
            if (readedGuids.guids.Contains(guid))
            {
                Console.WriteLine($"Hackathon was registered before");
                readWriteLock.ExitWriteLock();
            }
            else
            {
                readedGuids.guids.Add(guid);
                var hackathon = new Hackathon.Hackathon(teams, dataSaver);
                hackathon.Complete();
                var harmonicMean = hackathon.HarmonicMean;
                readWriteLock.ExitWriteLock();
                Console.WriteLine($"Harmonic mean: {harmonicMean}");
            }
        }
    }
}
using System.Diagnostics;
using Hackathon;
using Hackathon.DataProviders;
using HRManagerWebApp;
using MassTransit;
using MassTransitMessages.Messages;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HRDirectorWebApp.Controllers;

public class Teams(ILogger<Teams> logger, IDataSavingInterface dataSaver, JsonBodyReader reader, ReaderWriterLockSlim readWriteLock, ReadedGuids readedGuids, HRDirector hrDirector, IBus bus, IConfiguration configuration) : Controller
{
    public async Task Post()
    {
        var bodyStr = await reader.ReadJsonBody(Request);
        var hackathonTeams = JsonConvert.DeserializeObject<HackathonTeams>(bodyStr);
        var teams = hackathonTeams?.teams;
        var guid = hackathonTeams?.guid;
        logger.LogInformation("Got teams");
        if (teams != null)
        {
            Response.StatusCode = 200;
            await Response.WriteAsync("Ok");

            if (readedGuids.guids.Contains(guid!))
            {
                Console.WriteLine($"Hackathon was registered before");
            }
            else
            {
                readedGuids.guids.Add(guid!);
                if (hrDirector.IsEmployeesEnough() && !hrDirector.IsTriedToSave())
                {
                    hrDirector.SaveHackathon();
                    hrDirector.Reset();
                    if (hrDirector.GetHoldedHackathons() < Int32.Parse(configuration["NUMBER_OF_HACKATHONS"]))
                    {
                        await bus.Publish(new HackathonStarted() { HackathonId = hrDirector.GetHackathonId() });
                    }

                }
                Console.WriteLine($"Harmonic mean: {hrDirector.GetHarmonicMean()}");
            }
        }
        else
        {
            Response.StatusCode = 400;
            await Response.WriteAsync("Bad request");
        }
    }
}
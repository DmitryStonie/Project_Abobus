using System;
using Hackathon.Database.SQLite;
using Hackathon.DataProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Hackathon;

public class HackathonWorker(
    ITeamBuildingStrategy teamBuildingStrategy,
    IWishListGenerator wishListGenerator,
    IConfiguration configuration,
    IDataLoadingInterface dataLoader,
    IDataSavingInterface dataSaver,
    IDataInitializationInterface dataInitializator,
    int iterations = 0)
{
    private readonly int _employeesCount = Int32.Parse(configuration["EmployeesCount"]);
    private readonly int _iterationCount = iterations != 0 ? iterations : Int32.Parse(configuration["IterationCount"]);

    public void Run()
    {
        Console.WriteLine("HackathonWorker running...");
        var harmonicMeans = 0.0;
        var iterationCount = 0;
        dataInitializator.InitializeDatabase();
        while (iterationCount < _iterationCount)
        {
            var hackathon = new Hackathon(_employeesCount, teamBuildingStrategy, wishListGenerator, dataLoader,
                dataSaver);
            hackathon.Run();
            harmonicMeans += hackathon.HarmonicMean;
            iterationCount++;
        }

        Console.WriteLine($"Arithmetic mean: {harmonicMeans / iterationCount}");
    }
}
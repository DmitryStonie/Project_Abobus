using Hackathon.Database.SQLite;
using Hackathon.DataProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Hackathon;

public class ConsoleApplication(
    IHostApplicationLifetime appLifetime,
    ITeamBuildingStrategy teamBuildingStrategy,
    IWishListGenerator wishListGenerator,
    IConfiguration configuration,
    ApplicationContext context,
    IDataLoadingInterface dataLoader,
    IDataSavingInterface dataSaver,
    IDatabaseLoadingInterface databaseLoader,
    IDataInitializationInterface databaseInitializator) : IHostedService
{
    private bool _running = true;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(RunAsync);
        return Task.CompletedTask;
    }

    public void RunAsync()
    {
        Console.WriteLine("Welcome to HackathonApp!)");
        while (_running)
        {
            Console.WriteLine(" Please select what you would like to run:\n" +
                              "1: Run one random Hackathon\n" +
                              "2: Get info about Hackathon by id\n" +
                              "3: Get arithmetic mean of all Hackathons\n" +
                              "4: Run 1000 random Hackathons\n" +
                              "5: Exit");
            try
            {
                int intTemp = Convert.ToInt32(Console.ReadLine());
                if (intTemp is >= 1 and <= 4)
                {
                    UserDialog(intTemp);
                }
                else if (intTemp == 5)
                {
                    Console.WriteLine("Goodbye");
                    appLifetime.StopApplication();
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                appLifetime.StopApplication();
            }
        }
    }

    private void UserDialog(int intTemp)
    {
        switch (intTemp)
        {
            case 1:
                new HackathonWorker(teamBuildingStrategy, wishListGenerator, configuration, dataLoader, dataSaver,
                    databaseInitializator, 1).Run();
                break;
            case 2:
                Console.WriteLine("Enter hackathon id: ");
                int id = Convert.ToInt32(Console.ReadLine());
                var hackathonDto = databaseLoader.LoadHackathon(id);
                if (hackathonDto != null)
                {
                    PrintHackathon(id, hackathonDto);
                }
                else
                {
                    Console.WriteLine("Hackathon not found");
                }

                break;
            case 3:
                double? arithmeticMean = databaseLoader.LoadArithmeticMean();
                PrintArithmeticMean(arithmeticMean);
                break;
            case 4:
                new HackathonWorker(teamBuildingStrategy, wishListGenerator, configuration, dataLoader, dataSaver,
                    databaseInitializator, 1000).Run();
                break;
        }
    }

    private void PrintHackathon(in int id, HackathonDto hackathonDto)
    {
        Console.WriteLine($"Hackathon Id: {id}\nHackathon harmonic mean: {hackathonDto.HarmonicMean}\nHackathon teams:");
        foreach (var team in hackathonDto.Teams)
        {
            Console.WriteLine($"Team id: {team.Id}\t Junior: {team.Junior}\t TeamLead: {team.TeamLead}");
        }

        Console.WriteLine("hackathon participants");
        Console.WriteLine("juniors");
        foreach (var participant in hackathonDto.Juniors)
        {
            Console.WriteLine(participant);
        }
        Console.WriteLine("teamleads");
        foreach (var participant in hackathonDto.TeamLeads)
        {
            Console.WriteLine(participant);
        }
    }

    private void PrintArithmeticMean(double? arithmeticMean)
    {
        if (arithmeticMean != null)
        {
            Console.WriteLine($"Average harmonic mean: {arithmeticMean}");
        }
        else
        {
            Console.WriteLine("Hackathons not found");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask; // _running = false         
}
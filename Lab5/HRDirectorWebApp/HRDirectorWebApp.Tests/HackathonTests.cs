using Hackathon;
using Hackathon.DataProviders;
using Microsoft.Extensions.Configuration;

namespace HRDirectorWebApp.Tests;

public class HackathonTests
{
    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    public void HackathonStabilityTest_AllHarmonicMeansShouldBeSame()
    {
        //Arrange
        var dataLoader = new CsvDataLoader(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());
        var hackathon =
            new Hackathon.Hackathon(20, new TeamBuildingStrategy(), new SimpleWishListGenerator(), dataLoader);
        //Act
        hackathon.Run();
        var expectedHarmonicMean = hackathon.HarmonicMean;
        var harmonicMeans = new List<double>();
        for (var i = 0; i < 5; i++)
        {
            hackathon = new Hackathon.Hackathon(20, new TeamBuildingStrategy(), new SimpleWishListGenerator(),
                dataLoader);
            hackathon.Run();
            harmonicMeans.Add(hackathon.HarmonicMean);
        }

        //Assert
        foreach (var harmonicMean in harmonicMeans)
        {
            Assert.Less(harmonicMean - expectedHarmonicMean, 1e-9,
                $"Harmonic mean is different. At first time it was {expectedHarmonicMean}. At some time it's {harmonicMean}");
        }
    }
}
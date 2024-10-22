using Hackathon;

namespace Hackathon.Test;

public class HRDirectorTests
{
    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    public void SameNumbersHarmonicMeanTest_HarmonicMeanOfSameNumbersShouldBeNumber()
    {
        //Arrange
        var numberList = Enumerable.Repeat(10, 1000).ToList();
        var expected = 10.0;
        //Act
        var result = Math.Round(numberList.CountHarmonicMean(), 6);
        //Assert
        Assert.AreEqual(result, expected, $"Harmonic mean is invalid. Should be {expected}. Got {result}");
    }

    [Test]
    public void BigNumbersHarmonicMeanTest_HarmonicMeanOfBigNumbersShouldBeCorrect()
    {
        //Arrange
        var numberList = new List<int>()
        {
            209843298, 89329803, 982437432, 453743284, 23475324, 273473278, 224392759, 974327342, 178974334, 293478437
        };
        var expected = 125134669.9404428899;
        //Act
        var result = numberList.CountHarmonicMean();
        //Assert
        Assert.Less(expected - result, 1e-9, $"Harmonic mean is invalid. Should be {expected}. Got {result}");
    }

    [Test]
    public void ZeroDivisionHarmonicMeanTest_ShouldThrowDivideByZeroException()
    {
        var numberList = new List<int>();
        Assert.Catch<DivideByZeroException>(() => HarmonicMean.CountHarmonicMean(numberList));
    }

    [Test]
    public void DistributionStabilityTest_ShouldCountSameHarmonicMeans()
    {
        //Arrange
        var juniors = DataProvider.GetJuniors();
        var teamLeads = DataProvider.GetTeamLeads();
        juniors.ForEach(junior => junior.Wishlist = new Wishlist(teamLeads.Cast<Employee>().ToList()));
        teamLeads.ForEach(teamLead => teamLead.Wishlist = new Wishlist(juniors.Cast<Employee>().ToList()));
        var hrDirector = new HrDirector();
        var hrManager = new HrManager(juniors, teamLeads, new TeamBuildingStrategy());
        // Act
        var teams = hrManager.DistributeParticipants();
        var expectedHarmonicMean = hrDirector.CountHarmonicMean(teams);
        var harmonicMeans = new List<double>();
        for (int i = 0; i < 5; i++)
        {
            harmonicMeans.Add(hrDirector.CountHarmonicMean(teams));
        }

        // Assert
        foreach (var harmonicMean in harmonicMeans)
        {
            Assert.Less(harmonicMean - expectedHarmonicMean, 1e-9,
                "HrDirector counted 2 different harmonicMeans, but wishlists were the same.");
        }
    }
}
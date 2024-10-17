using Hackathon;
using Hackathon.DataProviders;
using Microsoft.Extensions.Configuration;
using Moq;

namespace HRManagerWebApp.Tests;

public class HRManagerTests
{
    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    public void TeamsSizeTest_ShouldBeCorrectAmountOfTeam()
    {
        //Arrange
        int teamsCount = 20;
        var juniors = DataProvider.GetJuniors();
        var teamLeads = DataProvider.GetTeamLeads();
        var generator = new RandomWishlistGenerator();
        juniors.ForEach(junior =>
            junior.Wishlist = new Wishlist(generator.CreateWishlist(teamLeads.Cast<Employee>().ToList())));
        teamLeads.ForEach(teamLead =>
            teamLead.Wishlist = new Wishlist(generator.CreateWishlist(juniors.Cast<Employee>().ToList())));
        var hrManager = new HrManager(juniors, teamLeads, new TeamBuildingStrategy());
        //Act
        var teams = hrManager.DistributeParticipants();
        //Assert
        if (teams is null)
        {
            Assert.Fail("Teams cannot be null.");
        }

        Assert.AreEqual(teams!.Count, teamsCount, "Number of teams were incorrect");
    }

    [Test]
    public void StrategyStabilityTest_ShouldGenerateSameTeams()
    {
        //Arrange
        var juniors = DataProvider.GetJuniors();
        var teamLeads = DataProvider.GetTeamLeads();
        juniors.ForEach(junior => junior.Wishlist = new Wishlist(teamLeads.Cast<Employee>().ToList()));
        teamLeads.ForEach(teamLead => teamLead.Wishlist = new Wishlist(juniors.Cast<Employee>().ToList()));
        var hrManager = new HrManager(juniors, teamLeads, new TeamBuildingStrategy());
        //Act
        var teamsExpected = hrManager.DistributeParticipants();
        Console.WriteLine(teamsExpected[0]);
        var teamsList = new List<List<Team>>();
        for (int i = 0; i < 5; i++)
        {
            juniors.ForEach(junior => junior.Wishlist.ResetCandidateSequence());
            teamLeads.ForEach(teamLead => teamLead.Wishlist.ResetCandidateSequence());
            teamsList.Add(hrManager.DistributeParticipants());
        }

        Console.WriteLine(teamsList[0][0]);
        //Assert
        foreach (var teams in teamsList)
        {
            for (int i = 0; i < teamsExpected.Count; i++)
            {
                Assert.AreEqual(teamsExpected[i], teams[i],
                    "HrManager provided 2 different distributions, but wishlists were the same.");
            }
        }
    }

    [Test]
    public void OneStrategyCallTest_BuildTeamsShouldBeCalledOnce()
    {
        //Arrange
        var mock = new Mock<ITeamBuildingStrategy>();
        mock.Setup(fb => fb.BuildTeams(It.IsAny<IEnumerable<Employee>>(), It.IsAny<IEnumerable<Employee>>(),
            It.IsAny<IEnumerable<Wishlist>>(), It.IsAny<IEnumerable<Wishlist>>())).Returns(DataProvider.GetTeamsFull());
        var dataLoader = new CsvDataLoader(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());
        var hackathon = new Hackathon.Hackathon(20, mock.Object, new RandomWishlistGenerator(), dataLoader);
        //Act
        hackathon.Run();
        //Assert
        mock.Verify(
            fb => fb.BuildTeams(It.IsAny<IEnumerable<Employee>>(), It.IsAny<IEnumerable<Employee>>(),
                It.IsAny<IEnumerable<Wishlist>>(), It.IsAny<IEnumerable<Wishlist>>()), Times.Once);
    }
}
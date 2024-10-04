using Hackathon;
using Hackathon.Database.SQLite;
using Hackathon.DataProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using IConfiguration = Castle.Core.Configuration.IConfiguration;

namespace Hackathon.Test;

public class DatabaseTests
{
    private AddDbContextFactory _addDbContextFactory;
    const double HarmonicMean = 10;
    const int HackathonId = 1;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseSqlite("DataSource=file::memory:?cache=shared").Options;
        _addDbContextFactory = new AddDbContextFactory(options);
    }

    [Test]
    public void WriteHackathonTest_ShouldWriteHackathonData()
    {
        //Arrange
        var context = _addDbContextFactory.CreateDbContext();
        var dataSaver = new SQLiteDataSaver(context);
        var hackathon = new Hackathon(HarmonicMean, HackathonId);
        InitTestData(out var juniors, out var teamLeads, out var teams, HackathonId);
        //Act and Assert
        Assert.DoesNotThrow(() => dataSaver.SaveData(juniors, teamLeads, teams, hackathon));
    }

    [Test]
    public void MultipleWriteHackathonTest_ShouldWriteMultipleHackathonData()
    {
        //Arrange
        var context = _addDbContextFactory.CreateDbContext();
        var dataSaver = new SQLiteDataSaver(context);
        var hackathon = new Hackathon(HarmonicMean, HackathonId);
        InitTestData(out var juniors, out var teamLeads, out var teams, HackathonId);
        //Act and Assert
        Assert.DoesNotThrow(() => dataSaver.SaveData(juniors, teamLeads, teams, hackathon));
        Assert.DoesNotThrow(() => dataSaver.SaveData(juniors, teamLeads, teams, hackathon));
        Assert.DoesNotThrow(() => dataSaver.SaveData(juniors, teamLeads, teams, hackathon));
    }

    [Test]
    public void ReadHackathonTest_ShouldReadHackathonData()
    {
        //Assert
        var context = _addDbContextFactory.CreateDbContext();
        var dataSaver = new SQLiteDataSaver(context);
        var dataLoader = new SQLiteDataLoader(context);
        const double harmonic = 10;
        const int hackathonId = 1;
        var hackathon = new Hackathon(harmonic, hackathonId);
        InitTestData(out var juniors, out var teamLeads, out var teamsToUpload, hackathonId);
        //Act
        dataSaver.SaveData(juniors, teamLeads, teamsToUpload, hackathon);
        dataLoader.LoadHackathon(1, out var loadedEmployees, out var loadedTeams, out var harmonicMean);
        //Assert
        Assert.That(harmonicMean, Is.EqualTo(harmonic), "Harmonic mean is not the same!");
        var employees = new Dictionary<int, Employee>();
        teamLeads.ForEach(t => employees.Add(t.Id, t));
        juniors.ForEach(j => employees.Add(j.Id, j));
        var loadedEmployeesDict = new Dictionary<int, Employee>();
        loadedEmployees.ForEach(e => loadedEmployeesDict.Add(e.Id, e));
        foreach (var (id, emp) in employees)
        {
            Assert.That(loadedEmployeesDict.ContainsKey(id), Is.EqualTo(true),
                $"Employee with id {id} does not exist!");
            Assert.That(emp.Id, Is.EqualTo(loadedEmployeesDict[id].Id),
                $"Id of loaded employee is not the same! Expected: {emp.Id}, got: {loadedEmployeesDict[id].Id}");
            Assert.That(emp.Name, Is.EqualTo(loadedEmployeesDict[id].Name),
                $"Name of loaded employee is not the same! Expected: {emp.Name}, got: {loadedEmployeesDict[id].Name}");
        }

        var teamsLoadedDict = new Dictionary<int, Team>();
        loadedTeams.ForEach(t => teamsLoadedDict.Add(t.Id, t));
        foreach (var team in teamsToUpload)
        {
            Assert.That(teamsLoadedDict.ContainsKey(team.Id), Is.EqualTo(true),
                $"Team with id {team.Id} does not exist!");
            Assert.That(team.HackathonId, Is.EqualTo(teamsLoadedDict[team.Id].HackathonId),
                $"HackathonId of loaded team is not the same! Expected: {team.HackathonId}, got: {teamsLoadedDict[team.Id].HackathonId}");
            Assert.That(team.TeamLeadId, Is.EqualTo(teamsLoadedDict[team.Id].TeamLeadId),
                $"TeamLeadId of loaded team is not the same! Expected: {team.TeamLeadId}, got: {teamsLoadedDict[team.Id].TeamLeadId}");
            Assert.That(team.JuniorId, Is.EqualTo(teamsLoadedDict[team.Id].JuniorId),
                $"JuniorId of loaded team is not the same! Expected: {team.JuniorId}, got: {teamsLoadedDict[team.Id].JuniorId}");
        }
    }

    [Test]
    public void GetArithmeticMeanTest_ShouldReadArithmeticMeanAndInShouldBeCorrect()
    {
        //Arrange
        var context = _addDbContextFactory.CreateDbContext();
        var dataSaver = new SQLiteDataSaver(context);
        var dataLoader = new SQLiteDataLoader(context);
        int iterations = 10;
        double result = 0.0;
        //Act
        for (int i = 1; i < iterations + 1; i++)
        {
            result += i;
            var hackathon = new Hackathon(i, i);
            InitTestData(out var juniors, out var teamLeads, out var teams, i);
            dataSaver.SaveData(juniors, teamLeads, teams, hackathon);
        }

        //Assert
        Assert.That(dataLoader.LoadArithmeticMean(), Is.EqualTo(result / iterations), "ArithmeticMean incorrect");
        Assert.Catch(() => CsvReader.ReadCsv<Employee>(""));
    }

    private void InitTestData(out List<Junior> juniors, out List<TeamLead> teamLeads, out List<Team> teams,
        int hackathonId)
    {
        juniors = new List<Junior>
        {
            new(1, "Юдин Адам"), new(2, "Яшина Яна"), new(3, "Никитина Вероника"),
            new(4, "Рябинин Александр"), new(5, "Ильин Тимофей")
        };
        teamLeads = new List<TeamLead>
        {
            new(1, "Филиппова Ульяна"), new(2, "Николаев Григорий"),
            new(3, "Андреева Вероника"), new(4, "Коротков Михаил"),
            new(5, "Кузнецов Александр")
        };
        teams = new List<Team>();
        var juniorsList = juniors.Cast<Employee>().ToList();
        var teamLeadsList = teamLeads.Cast<Employee>().ToList();
        foreach (var junior in juniors)
        {
            junior.Wishlist = new Wishlist(teamLeadsList);
        }

        foreach (var teamlead in teamLeads)
        {
            teamlead.Wishlist = new Wishlist(juniorsList);
        }

        for (int i = 0; i < juniors.Count(); i++)
        {
            var team = new Team(juniors[i], teamLeads[i]);
            teams.Add(team);
        }
    }
}
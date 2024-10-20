using Hackathon;
using Hackathon.DataProviders;
using Hackathon.Migrations;

namespace HRManagerWebApp;

public class HRManager
{
    private Dictionary<int, Junior> _juniors;
    private Dictionary<int, TeamLead> _teamLeads;
    private Hackathon.Hackathon _hackathon;
    private List<Team>? _teams;
    private readonly int _employeeCount;

    public bool TeamsGenerated = false;
    public string guid;

    private readonly ITeamBuildingStrategy _teamBuildingStrategy;
    private readonly IDataSavingInterface _dataSaver;
    private readonly IDatabaseLoadingInterface _dataLoader;
    private readonly ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

    public HRManager(IConfiguration configuration, ITeamBuildingStrategy teamBuildingStrategy,
        IDataSavingInterface dataSaver, IDatabaseLoadingInterface databaseLoadingInterface)
    {
        _teamBuildingStrategy = teamBuildingStrategy;
        _dataSaver = dataSaver;
        _dataLoader = databaseLoadingInterface;
        _employeeCount = int.Parse(configuration["EMPLOYEES_COUNT"]!);
        guid = Guid.NewGuid().ToString();
        InitData();
    }

    private void InitData()
    {
        _readWriteLock.EnterWriteLock();
        var hackathonDto = _dataLoader.LoadLastHackathon();
        Console.WriteLine($"Get hackathon with {hackathonDto?.HarmonicMean}");
        if (hackathonDto == null || hackathonDto.HarmonicMean != 0.0)
        {
            _hackathon = new Hackathon.Hackathon();
            _juniors = new Dictionary<int, Junior>();
            _teamLeads = new Dictionary<int, TeamLead>();
            _teams = new List<Team>();
            Console.WriteLine("----------------------------- empty hackathon -------------------------");
        }
        else
        {
            _hackathon = new Hackathon.Hackathon(hackathonDto.HarmonicMean, hackathonDto.Id);
            _juniors = hackathonDto.Juniors.Select((s, index) => new { s, index })
                .ToDictionary(x => x.index, x => x.s);
            _teamLeads = hackathonDto.TeamLeads.Select((s, index) => new { s, index })
                .ToDictionary(x => x.index, x => x.s);
            _teams = hackathonDto.Teams;
            Console.WriteLine($"hackathon with {_juniors.Count}  {_teams.Count}");
        }

        _readWriteLock.ExitWriteLock();
    }

    public IEnumerable<Team>? GetTeams()
    {
        Console.WriteLine("----------------------------------try to get lock on teams-------------------------------");
        _readWriteLock.EnterWriteLock();
        Console.WriteLine("------------------------------------got lock-----------------------------------");
        if (TeamsGenerated == true)
        {
            _readWriteLock.ExitWriteLock();
            return new List<Team>(_teams);
        }

        Console.WriteLine("Creating...");
        _teams = CreateTeams().ToList();
        TeamsGenerated = true;
        _readWriteLock.ExitWriteLock();
        return new List<Team>(_teams);
    }

    private List<Team> CreateTeams()
    {
        Console.WriteLine("In create teams...");
        var juniorsWishLists = new List<Wishlist>();
        var teamLeadsWishLists = new List<Wishlist>();
        var juniors = new List<Junior>();
        var teamLeads = new List<TeamLead>();
        foreach (var junior in _juniors.Values)
        {
            juniors.Add(junior);
            juniorsWishLists.Add(junior.Wishlist);
        }

        foreach (var teamLead in _teamLeads.Values)
        {
            teamLeads.Add(teamLead);
            teamLeadsWishLists.Add(teamLead.Wishlist);
        }

        Console.WriteLine("Creating teams...");
        foreach (var team in _juniors)
        {
            Console.WriteLine(team);
        }

        foreach (var team in _teamLeads)
        {
            Console.WriteLine(team);
        }

        var teams = _teamBuildingStrategy.BuildTeams(teamLeads, juniors, teamLeadsWishLists, juniorsWishLists).ToList();
        Console.WriteLine("Saving teams...");
        _dataSaver.SaveData(_juniors.Values.ToList(), _teamLeads.Values.ToList(), teams, _hackathon);
        Console.WriteLine("Saved teams...");
        return teams;
    }

    public bool AddJunior(Junior junior)
    {
        _readWriteLock.EnterWriteLock();
        if (_juniors.ContainsKey(junior.JuniorId))
        {
            _readWriteLock.ExitWriteLock();
            return false;
        }

        junior.Wishlist.InitWishlistById(junior.JuniorId);
        _juniors[junior.JuniorId] = junior;
        _dataSaver.SaveEmployees(_juniors.Values.ToList(), _teamLeads.Values.ToList(), _hackathon);
        Console.WriteLine(
            $"----------------------------------- jun {_juniors.Count} lead {_teamLeads.Count} ----------------------------------");
        _readWriteLock.ExitWriteLock();
        return true;
    }

    public bool AddTeamLead(TeamLead teamLead)
    {
        _readWriteLock.EnterWriteLock();
        if (_teamLeads.ContainsKey(teamLead.TeamLeadId))
        {
            _readWriteLock.ExitWriteLock();
            return false;
        }

        teamLead.Wishlist.InitWishlistById(teamLead.TeamLeadId);
        _teamLeads[teamLead.TeamLeadId] = teamLead;
        _dataSaver.SaveEmployees(_juniors.Values.ToList(), _teamLeads.Values.ToList(), _hackathon);
        Console.WriteLine(
            $"----------------------------------- jun {_juniors.Count} lead {_teamLeads.Count} ----------------------------------");
        _readWriteLock.ExitWriteLock();
        return true;
    }

    public bool IsEmployeesEnough()
    {
        _readWriteLock.EnterReadLock();
        Console.WriteLine($"{_juniors.Count}   {_teamLeads.Count}   {_employeeCount}");
        bool result = _juniors.Count + _teamLeads.Count == _employeeCount;
        Console.WriteLine($"{result}");
        _readWriteLock.ExitReadLock();
        return result;
    }

    public void Reset()
    {
        _readWriteLock.EnterWriteLock();
        _hackathon.Complete();
        _readWriteLock.ExitWriteLock();
        guid = Guid.NewGuid().ToString();
        InitData();
    }
}
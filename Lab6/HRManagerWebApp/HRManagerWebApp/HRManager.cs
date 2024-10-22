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
    public int _hackathonId { get; set; }

    public bool TriedToSend = false;
    public bool TeamsGenerated = false;
    public string guid;

    private readonly ITeamBuildingStrategy _teamBuildingStrategy;
    private readonly IDataSavingInterface _dataSaver;
    private readonly IDatabaseLoadingInterface _dataLoader;
    private readonly ReaderWriterLockSlim _readWriteLock = new();
    private readonly object _sendLock = new();

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
        if (hackathonDto == null || hackathonDto.HarmonicMean != 0.0)
        {
            Console.WriteLine("Null hackathon");
            _hackathon = new Hackathon.Hackathon();
            _juniors = new Dictionary<int, Junior>();
            _teamLeads = new Dictionary<int, TeamLead>();
            _teams = new List<Team>();
        }
        else
        {
            Console.WriteLine("Existed hackathon");
            _hackathon = new Hackathon.Hackathon(hackathonDto.HarmonicMean, hackathonDto.Id);
            _juniors = hackathonDto.Juniors.Select((s, index) => new { s, index })
                .ToDictionary(x => x.index, x => x.s);
            _teamLeads = hackathonDto.TeamLeads.Select((s, index) => new { s, index })
                .ToDictionary(x => x.index, x => x.s);
            _teams = hackathonDto.Teams;
        }

        _readWriteLock.ExitWriteLock();
    }

    public IEnumerable<Team>? GetTeams()
    {
        _readWriteLock.EnterWriteLock();
        if (TeamsGenerated)
        {
            _readWriteLock.ExitWriteLock();
            return new List<Team>(_teams);
        }

        _teams = CreateTeams().ToList();
        TeamsGenerated = true;
        _readWriteLock.ExitWriteLock();
        return new List<Team>(_teams);
    }

    private List<Team> CreateTeams()
    {
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
        var teams = _teamBuildingStrategy.BuildTeams(teamLeads, juniors, teamLeadsWishLists, juniorsWishLists).ToList();
        _dataSaver.SaveData(_juniors.Values.ToList(), _teamLeads.Values.ToList(), teams, _hackathon);
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

        var employee = new Junior(junior.JuniorId, junior.Name, junior.Wishlist);
        employee.Wishlist.InitWishlistById(employee.JuniorId);
        _juniors[employee.JuniorId] = employee;
        _dataSaver.SaveEmployees(_juniors.Values.ToList(), _teamLeads.Values.ToList(), _hackathon);
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

        var employee = new TeamLead(teamLead.TeamLeadId, teamLead.Name, teamLead.Wishlist);
        employee.Wishlist.InitWishlistById(employee.TeamLeadId);
        _teamLeads[employee.TeamLeadId] = employee;
        _dataSaver.SaveEmployees(_juniors.Values.ToList(), _teamLeads.Values.ToList(), _hackathon);
        _readWriteLock.ExitWriteLock();
        return true;
    }

    public void SetHackathonId(int hackathonId)
    {
        _readWriteLock.EnterWriteLock();
        _hackathonId = hackathonId;
        _readWriteLock.ExitWriteLock();
    }

    public bool IsEmployeesEnough()
    {
        _readWriteLock.EnterWriteLock();
        Console.WriteLine($"Got {_juniors.Count} jun and {_teamLeads.Count} teamleads {TriedToSend}");
        bool result = _juniors.Count + _teamLeads.Count == _employeeCount;
        _readWriteLock.ExitWriteLock();
        return result;
    }

    public bool IsTriedToSend()
    {
        _readWriteLock.EnterWriteLock();
        if (TriedToSend)
        {
            _readWriteLock.ExitWriteLock();
            return true;
        }
        TriedToSend = true;
        _readWriteLock.ExitWriteLock();
        return false;
    }

    public void Reset()
    {
        guid = Guid.NewGuid().ToString();
        TriedToSend = false;
        InitData();
    }
}
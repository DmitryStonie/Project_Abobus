using Hackathon;
using Hackathon.DataProviders;

namespace HRDirectorWebApp;

public class HRDirector
{
    private Dictionary<int, Junior> _juniors = new();
    private Dictionary<int, TeamLead> _teamLeads = new();
    private Hackathon.Hackathon _hackathon = new();
    private List<Team> _teams = new();
    private int _hackathonId;
    private double _harmonicMean;
    private int _holdedHackathons = 0;

    public bool TriedToSave = false;
    public bool TeamsGenerated = false;

    private readonly ITeamBuildingStrategy _teamBuildingStrategy;
    private readonly IDataSavingInterface _dataSaver;
    private readonly IDatabaseLoadingInterface _dataLoader;
    private readonly ReaderWriterLockSlim _readWriteLock = new();

    public HRDirector(ITeamBuildingStrategy teamBuildingStrategy,
        IDataSavingInterface dataSaver, IDatabaseLoadingInterface databaseLoadingInterface)
    {
        _teamBuildingStrategy = teamBuildingStrategy;
        _dataSaver = dataSaver;
        _dataLoader = databaseLoadingInterface;
        SetNewHackathonId();
    }

    private bool SetNewHackathonId()
    {
        _readWriteLock.EnterWriteLock();
        var lastHackathon = _dataLoader.LoadLastHackathon();
        if (lastHackathon != null)
        {
            _hackathonId = lastHackathon.Id + 1;
            _readWriteLock.ExitWriteLock();
            return true;
        }
        _readWriteLock.ExitWriteLock();
        return false;
    }

    public int GetHackathonId()
    {
        return _hackathonId;
    }

    public int GetHoldedHackathons()
    {
        return _holdedHackathons;
    }

    public double GetHarmonicMean()
    {
        return _harmonicMean;
    }

    public void AddJunior(Junior junior)
    {
        _readWriteLock.EnterWriteLock();
        _juniors.Add(junior.Id, junior);
        _readWriteLock.ExitWriteLock();
    }

    public void AddTeamLead(TeamLead teamLead)
    {
        _readWriteLock.EnterWriteLock();
        _teamLeads.Add(teamLead.Id, teamLead);
        _readWriteLock.ExitWriteLock();
    }

    public bool IsEmployeesEnough()
    {
        _readWriteLock.EnterWriteLock();
        bool result = (_juniors.Count + _teamLeads.Count == _teams.Count * 2) && _teams.Count > 0;
        _readWriteLock.ExitWriteLock();
        return result;
    }

    public bool IsTriedToSave()
    {
        _readWriteLock.EnterWriteLock();
        if (TriedToSave)
        {
            _readWriteLock.ExitWriteLock();
            return true;
        }
        TriedToSave = true;
        _readWriteLock.ExitWriteLock();
        return false;
    }

    public void Reset()
    {
        TriedToSave = false;
        SetNewHackathonId();
    }

    public void SaveHackathon()
    {
        _readWriteLock.EnterWriteLock();
        _hackathon = new Hackathon.Hackathon(0, _hackathonId);
        _hackathon.Complete();
        _harmonicMean = _hackathon.HarmonicMean;
        _holdedHackathons += 1;
        _readWriteLock.ExitWriteLock();
    }
}
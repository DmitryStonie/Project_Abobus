using Hackathon;

namespace HRManagerWebApp;

public class HRManager(ITeamBuildingStrategy teamBuildingStrategy)
{
    private readonly Dictionary<int, Junior> _juniors = new();
    private readonly Dictionary<int, TeamLead> _teamLeads = new();

    public IEnumerable<Team> CreateTeams()
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
        var teams = teamBuildingStrategy.BuildTeams(teamLeads, juniors, teamLeadsWishLists, juniorsWishLists);
        return teams;
    }
    
    public bool AddJunior(Junior junior)
    {
        if (_juniors.ContainsKey(junior.JuniorId))
        {
            return false;
        }

        junior.Wishlist.InitWishlist();
        _juniors[junior.JuniorId] = junior;
        return true;
    }

    public bool AddTeamLead(TeamLead teamLead)
    {
        if (_teamLeads.ContainsKey(teamLead.TeamLeadId))
        {
            return false;
        }

        teamLead.Wishlist.InitWishlist();
        _teamLeads[teamLead.TeamLeadId] = teamLead;
        return true;
    }

    public int GetJuniorsCount()
    {
        return _juniors.Count;
    }
    public int GetTeamleadsCount()
    {
        return _teamLeads.Count;
    }

    public void ClearEmployees()
    {
        _juniors.Clear();
        _teamLeads.Clear();
    }
}
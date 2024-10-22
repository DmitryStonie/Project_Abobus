namespace Hackathon;

public class TeamBuildingStrategy : ITeamBuildingStrategy
{
    private Dictionary<Employee, Candidate?> _teamLeadsOffers;
    private Dictionary<Employee, Candidate?> _juniorsOffers;

    class Candidate(Employee employee, int score)
    {
        public const int DefaultScore = 0;
        public Employee Employee { get; } = employee;
        public int Score { get; } = score;
    }

    private void InitOffers(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors)
    {
        _juniorsOffers = new Dictionary<Employee, Candidate?>();
        _teamLeadsOffers = new Dictionary<Employee, Candidate?>();
        foreach (var junior in juniors)
        {
            _juniorsOffers.Add(junior, null);
        }

        foreach (var teamLead in teamLeads)
        {
            _teamLeadsOffers.Add(teamLead, null);
        }
    }

    public IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists)
    {
        var juniorsWithoutTeam = new List<Employee>(juniors);
        InitOffers(teamLeads, juniorsWithoutTeam);
        while (juniorsWithoutTeam.Count > 0)
        {
            var rejectedJuniors = new List<Employee>();
            foreach (var junior in juniorsWithoutTeam)
            {
                var possibleTeamLead = junior.Wishlist.GetNextCandidate();
                if (possibleTeamLead is null)
                {
                    continue;
                }

                if (_teamLeadsOffers.ContainsKey(possibleTeamLead))
                {
                    possibleTeamLead = teamLeads.FirstOrDefault(teamLead => teamLead.Equals(possibleTeamLead));
                    if (_teamLeadsOffers[possibleTeamLead] is null)
                    {
                        _juniorsOffers[junior] = new Candidate(possibleTeamLead, Candidate.DefaultScore);
                        _teamLeadsOffers[possibleTeamLead] = new Candidate(junior, possibleTeamLead.GetScore(junior));
                    }
                    else if (possibleTeamLead.GetScore(junior) > _teamLeadsOffers[possibleTeamLead]?.Score)
                    {
                        _juniorsOffers[_teamLeadsOffers[possibleTeamLead]!.Employee] = null;
                        rejectedJuniors.Add(_teamLeadsOffers[possibleTeamLead]!.Employee);
                        _juniorsOffers[junior] = new Candidate(possibleTeamLead, Candidate.DefaultScore);
                        _teamLeadsOffers[possibleTeamLead] = new Candidate(junior, possibleTeamLead.GetScore(junior));
                    }
                    else
                    {
                        rejectedJuniors.Add(junior);
                    }
                }
                else
                {
                    rejectedJuniors.Add(junior);
                }

                juniorsWithoutTeam = rejectedJuniors.ToList();
            }
        }

        var teams = new List<Team>();
        foreach (var (junior, candidate) in _juniorsOffers)
        {
            if (candidate != null)
            {
                teams.Add(new Team((Junior)junior, (TeamLead)candidate.Employee));
            }
        }

        return teams;
    }
}
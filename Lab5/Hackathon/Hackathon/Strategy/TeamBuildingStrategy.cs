namespace Hackathon;

public class TeamBuildingStrategy : ITeamBuildingStrategy
{
    private Dictionary<int, Candidate?> _teamLeadsOffers;
    private Dictionary<int, Candidate?> _juniorsOffers;

    class Candidate(Employee employee, int score)
    {
        public const int DefaultScore = 0;
        public Employee Employee { get; } = employee;
        public int Score { get; } = score;
    }

    private void InitOffers(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors)
    {
        _juniorsOffers = new Dictionary<int, Candidate?>();
        _teamLeadsOffers = new Dictionary<int, Candidate?>();
        foreach (var junior in juniors)
        {
            _juniorsOffers.Add(junior.JuniorId, null);
        }

        foreach (var teamLead in teamLeads)
        {
            _teamLeadsOffers.Add(teamLead.TeamLeadId, null);
        }
    }

    public IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists)
    {
        var juniorsWithoutTeam = new List<Employee>(juniors);
        InitOffers(teamLeads, juniorsWithoutTeam);
        
        foreach (var teamlead in teamLeads)
        {
            Console.WriteLine(teamlead);
            foreach (var wish in teamlead.Wishlist.Wishes)
            {
                Console.WriteLine($"{wish.OwnerId}  {wish.PartnerId}  {wish.WishlistId} {wish.Score}");
            }
            foreach (var wish in teamlead.Wishlist._wishlistDictionary)
            {
                Console.WriteLine($"{wish.Key}   {wish.Value}");
            }
        }
        foreach (var teamlead in juniors)
        {
            Console.WriteLine(teamlead);
            foreach (var wish in teamlead.Wishlist.Wishes)
            {
                Console.WriteLine($"{wish.OwnerId}  {wish.PartnerId}  {wish.WishlistId} {wish.Score}");
            }
            foreach (var wish in teamlead.Wishlist._wishlistDictionary)
            {
                Console.WriteLine($"{wish.Key}   {wish.Value}");
            }
        }
        
        while (juniorsWithoutTeam.Count > 0)
        {
            var rejectedJuniors = new List<Employee>();
            foreach (var junior in juniorsWithoutTeam)
            {
                var possibleTeamLead = junior.Wishlist.GetNextCandidate();
                Console.WriteLine("-------------" + possibleTeamLead?.ToString());
                if (possibleTeamLead is null)
                {
                    continue;
                }

                if (_teamLeadsOffers.ContainsKey(possibleTeamLead.TeamLeadId))
                {
                    Console.WriteLine("Contains");
                    possibleTeamLead = teamLeads.FirstOrDefault(teamLead => teamLead.Equals(possibleTeamLead));
                    if (_teamLeadsOffers[possibleTeamLead.TeamLeadId] is null)
                    {
                        Console.WriteLine("null");
                        _juniorsOffers[junior.JuniorId] = new Candidate(possibleTeamLead, Candidate.DefaultScore);
                        Console.WriteLine("null 2");
                        var offer = _teamLeadsOffers[possibleTeamLead.TeamLeadId];
                        Console.WriteLine("null 2.5");
                        var score = possibleTeamLead.GetScore(junior);
                        Console.WriteLine("null 2.75");
                        _teamLeadsOffers[possibleTeamLead.TeamLeadId] = new Candidate(junior, score);
                        Console.WriteLine("null 3");
                    }
                    else if (possibleTeamLead.GetScore(junior) > _teamLeadsOffers[possibleTeamLead.TeamLeadId]?.Score)
                    {
                        Console.WriteLine("not null");
                        _juniorsOffers[_teamLeadsOffers[possibleTeamLead.TeamLeadId]!.Employee.JuniorId] = null;
                        rejectedJuniors.Add(_teamLeadsOffers[possibleTeamLead.TeamLeadId]!.Employee);
                        _juniorsOffers[junior.JuniorId] = new Candidate(possibleTeamLead, Candidate.DefaultScore);
                        _teamLeadsOffers[possibleTeamLead.TeamLeadId] = new Candidate(junior, possibleTeamLead.GetScore(junior));
                    }
                    else
                    {
                        Console.WriteLine("rejected");
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
                var jun = juniors.FirstOrDefault(j => j.JuniorId == junior);
                teams.Add(new Team((Junior)jun!, (TeamLead)candidate.Employee));
            }
        }

        return teams;
    }
}
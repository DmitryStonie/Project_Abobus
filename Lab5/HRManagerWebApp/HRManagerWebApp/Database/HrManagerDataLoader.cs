using Castle.Core.Configuration;
using Hackathon.Database.SQLite;

namespace Hackathon.DataProviders;

public class HrManagerDataLoader(HrManagerApplicationContext context) : IDataLoadingInterface, IDatabaseLoadingInterface
{
    public List<Junior> LoadJuniors()
    {
        return context.Juniors.ToList();
    }

    public List<TeamLead> LoadTeamLeads()
    {
        return context.Teamleads.ToList();
    }

    public HackathonDto? LoadHackathon(int id)
    {
        var hackathon = context.Hackathons.FirstOrDefault(h => h.Id == id);
        var teams = new List<Team>();
        var juniors = new List<Junior>();
        var teamleads = new List<TeamLead>();
        var harmonicMean = 0.0;
        if (hackathon != null)
        {
            harmonicMean = hackathon.HarmonicMean;
            var teamsQuery = context.Teams.Where(t => t.HackathonId == hackathon.Id);
            teams = teamsQuery.ToList();
            var juniorsDict = teamsQuery
                .Join(context.Juniors, team => team.JuniorId, junior => junior.Id,
                    (team, junior) => new { junior }).ToDictionary(x => x.junior.Id, x => x.junior);
            var teamLeadsDict = teamsQuery
                .Join(context.Teamleads, team => team.TeamLeadId, teamLead => teamLead.Id,
                    (team, teamLead) => new { teamLead }).ToDictionary(x => x.teamLead.Id, x => x.teamLead);
            juniors.AddRange(juniorsDict.Values);
            teamleads.AddRange(teamLeadsDict.Values);
            foreach (var team in teamsQuery)
            {
                team.Junior = juniorsDict[team.JuniorId];
                team.TeamLead = teamLeadsDict[team.TeamLeadId];
            }

            return new HackathonDto(){Id = id, HarmonicMean = harmonicMean, Juniors = juniors, TeamLeads = teamleads, Teams = teams};
        }
        return null;
    }

    public double? LoadArithmeticMean()
    {
        var hackathons = context.Hackathons;
        double sum = 0;
        foreach (var hackathon in hackathons)
        {
            sum += hackathon.HarmonicMean;
        }

        if (hackathons.Any())
        {
            return sum / hackathons.Count();
        }

        return null;
    }

    public HackathonDto? LoadLastHackathon()
    {
        var hackathon = context.Hackathons.FirstOrDefault(h => h.Id == context.Hackathons.Max(h => h.Id));
        Console.WriteLine($"Got hackathon {hackathon?.Id}");
        if (hackathon != null && hackathon.Id < 1) return LoadHackathon(hackathon.Id);
        return null;
    }
}
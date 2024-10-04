using Castle.Core.Configuration;
using Hackathon.Database.SQLite;

namespace Hackathon.DataProviders;

public class SQLiteDataLoader(ApplicationContext context) : IDataLoadingInterface, IDatabaseLoadingInterface
{
    public List<Junior> LoadJuniors()
    {
        return context.Juniors.ToList();
    }

    public List<TeamLead> LoadTeamLeads()
    {
        return context.Teamleads.ToList();
    }

    public bool LoadHackathon(int id, out List<Employee> employees, out List<Team> teams, out double harmonicMean)
    {
        var hackathon = context.Hackathons.FirstOrDefault(h => h.Id == id);
        teams = new List<Team>();
        employees = new List<Employee>();
        harmonicMean = 0;
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
            employees.AddRange(juniorsDict.Values);
            employees.AddRange(teamLeadsDict.Values);
            foreach (var team in teamsQuery)
            {
                team.Junior = juniorsDict[team.JuniorId];
                team.TeamLead = teamLeadsDict[team.TeamLeadId];
            }

            return true;
        }

        return false;
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
}
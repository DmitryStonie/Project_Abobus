using Hackathon.Database.SQLite;
using Microsoft.Extensions.Configuration;

namespace Hackathon.DataProviders;

public class DataInitializator(ApplicationContext context, IConfiguration configuration)
    : IDataInitializationInterface
{
    public void InitializeDatabase()
    {
        var juniors = CsvReader.ReadCsv<Junior>(configuration["JuniorsPath"]!);
        var teamLeads = CsvReader.ReadCsv<TeamLead>(configuration["TeamLeadsPath"]!);
        SaveJuniors(juniors);
        SaveTeamLeads(teamLeads);
        context.SaveChanges();
    }

    private void SaveJuniors(List<Junior> juniors)
    {
        foreach (var junior in juniors)
        {
            if (!context.Juniors.Any(j => j.JuniorId == junior.JuniorId))
                context.Juniors.Add(junior);
        }
    }

    private void SaveTeamLeads(List<TeamLead> teamLeads)
    {
        foreach (var teamLead in teamLeads)
        {
            if (!context.Teamleads.Any(t => t.TeamLeadId == teamLead.TeamLeadId))
                context.Teamleads.Add(teamLead);
        }
    }
}
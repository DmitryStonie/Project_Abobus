using Microsoft.Extensions.Configuration;

namespace Hackathon.DataProviders;

public class CsvDataLoader(IConfiguration configuration) : IDataLoadingInterface
{
    private List<Junior>? juniors;
    private List<TeamLead>? teamleads;
    public List<Junior> LoadJuniors()
    {
        if (juniors is null)
        {
            juniors = CsvReader.ReadCsv<Junior>(configuration["JuniorsPath"]!);
        }
        return juniors!;
    }

    public List<TeamLead> LoadTeamLeads()
    {
        if (teamleads is null)
        {
            teamleads = CsvReader.ReadCsv<TeamLead>(configuration["TeamLeadsPath"]!);
        }
        return teamleads!;
    }
}
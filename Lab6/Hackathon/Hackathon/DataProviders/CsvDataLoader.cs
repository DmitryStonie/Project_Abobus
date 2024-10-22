using Microsoft.Extensions.Configuration;

namespace Hackathon.DataProviders;

public class CsvDataLoader(IConfiguration configuration) : IDataLoadingInterface
{
    public List<Junior> LoadJuniors()
    {
        return CsvReader.ReadCsv<Junior>(configuration["JuniorsPath"]!);
    }

    public List<TeamLead> LoadTeamLeads()
    {
        return CsvReader.ReadCsv<TeamLead>(configuration["TeamLeadsPath"]!);
    }
}
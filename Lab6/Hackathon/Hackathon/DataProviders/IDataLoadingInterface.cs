using Castle.Core.Configuration;

namespace Hackathon.DataProviders;

public interface IDataLoadingInterface
{
    public List<Junior> LoadJuniors();
    public List<TeamLead> LoadTeamLeads();
}
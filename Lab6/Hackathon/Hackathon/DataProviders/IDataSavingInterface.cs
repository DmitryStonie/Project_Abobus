namespace Hackathon.DataProviders;

public interface IDataSavingInterface
{
    public void SaveData(List<Junior> juniors, List<TeamLead> teamLeads, List<Team> teams, Hackathon hackathon);
    public void SaveEmployees(List<Junior> juniors, List<TeamLead> teamLeads, Hackathon hackathon);
}
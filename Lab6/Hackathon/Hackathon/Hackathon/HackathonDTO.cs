namespace Hackathon;

public class HackathonDto
{
    public int Id { get; set; }
    public List<TeamLead> TeamLeads { get; set; }
    public List<Junior> Juniors { get; set; }
    public List<Team> Teams { get; set; }
    public  double HarmonicMean { get; set; }
}
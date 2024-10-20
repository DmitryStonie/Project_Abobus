namespace Hackathon;

public class Team
{
    public TeamLead TeamLead { get; set; }

    public Junior Junior { get; set; }

    public int Id { get; set; }
    public int HackathonId { get; set; }
    public int JuniorId { get; set; }

    public int TeamLeadId { get; set; }

    public Team()
    {
        TeamLead = new TeamLead();
        Junior = new Junior();
    }

    public Team(Junior junior, TeamLead teamLead)
    {
        TeamLead = teamLead;
        Junior = junior;
    }

    public int GetJuniorScore()
    {
        return TeamLead.GetScore(Junior);
    }

    public int GetTeamLeadScore()
    {
        return Junior.GetScore(TeamLead);
    }

    public override bool Equals(object other)
    {
        if (other is Team team)
            return Junior == team.Junior && TeamLead == team.TeamLead;
        return false;
    }
}
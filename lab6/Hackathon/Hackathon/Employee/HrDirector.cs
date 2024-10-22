namespace Hackathon
{
    public class HrDirector
    {
        public List<Junior> Juniors { get; set; }
        public List<TeamLead> Teamleads { get; set; }
        public List<Team> Teams { get; set; }

        public HrDirector()
        {
            Juniors = new List<Junior>();
            Teamleads = new List<TeamLead>();
            Teams = new List<Team>();
        }
        public double CountHarmonicMean(List<Team> teams)
        {
            var scores = new List<int>();
            foreach (var team in teams)
            {
                scores.Add(team.GetJuniorScore());
                scores.Add(team.GetTeamLeadScore());
            }

            return scores.CountHarmonicMean();
        }
    }
}
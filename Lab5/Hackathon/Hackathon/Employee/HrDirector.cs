namespace Hackathon
{
    public class HrDirector
    {
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
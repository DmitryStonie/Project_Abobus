namespace Hackathon
{
    public class HrManager(List<Junior> juniors, List<TeamLead> teamLeads, ITeamBuildingStrategy teamBuildingStrategy)
    {
        public List<Junior> Juniors = new List<Junior>();
        public List<TeamLead> TeamLeads = new List<TeamLead>();
        private List<Wishlist> GetWishes(List<Employee> employees)
        {
            var wishes = new List<Wishlist>();
            foreach (var employee in employees)
            {
                wishes.Add(employee.Wishlist);
            }

            return wishes;
        }

        private List<Wishlist> GetTeamLeadsWishes()
        {
            return GetWishes(teamLeads.Cast<Employee>().ToList());
        }

        private List<Wishlist> GetJuniorsWishes()
        {
            return GetWishes(juniors.Cast<Employee>().ToList());
        }

        public List<Team> DistributeParticipants()
        {
            if (juniors.Count == 0 || teamLeads.Count == 0 || juniors.Count != teamLeads.Count)
            {
                throw new HrManagerDistributionException("Invalid number of Juniors and TeamLeads");
            }

            return teamBuildingStrategy.BuildTeams(teamLeads, juniors, GetTeamLeadsWishes(), GetJuniorsWishes())
                .ToList();
        }
    }
}
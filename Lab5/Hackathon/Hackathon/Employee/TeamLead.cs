namespace Hackathon
{
    public class TeamLead : Employee
    {
        public TeamLead()
        {
        }

        public TeamLead(int teamLeadId, string? name) : base(name)
        {
            TeamLeadId = teamLeadId;
        }

        public TeamLead(int teamLeadId, string? name, int id, Wishlist wishlist) : base(name, id, wishlist)
        {
            TeamLeadId = teamLeadId;
        }
    }
}
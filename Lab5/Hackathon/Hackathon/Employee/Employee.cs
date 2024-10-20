namespace Hackathon
{
    public class Employee
    {
        public Wishlist Wishlist { get; set; }

        public int Id { get; set; }
        public string? Name { get; init; }
        public int TeamLeadId { get; init; }
        public int JuniorId { get; init; }

        public Employee()
        {
        }

        public Employee(string? name)
        {
            Name = name;
        }

        public Employee(string? name, int id, Wishlist wishlist)
        {
            Name = name;
            Id = id;
            Wishlist = wishlist;
        }


        public int GetScore(Employee participant)
        {
            return Wishlist.GetScore(participant);
        }

        public override string ToString() => Id.ToString() + " - " + Name;

        public override bool Equals(object other)
        {
            if (other is Employee employee)
                return Id == employee.Id && JuniorId == employee.JuniorId && Name == employee.Name &&
                       TeamLeadId == employee.TeamLeadId;
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^
                   Id.GetHashCode() ^
                   JuniorId.GetHashCode() ^
                   TeamLeadId.GetHashCode();
        }
    }
}
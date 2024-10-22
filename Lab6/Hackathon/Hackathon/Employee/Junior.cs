namespace Hackathon
{
    public class Junior : Employee
    {
        public Junior()
        {
        }

        public Junior(int juniorId, string? name) : base(name)
        {
            JuniorId = juniorId;
        }
        public Junior(int juniorId, string? name, Wishlist wishlist) : base(name, juniorId, wishlist)
        {
            JuniorId = juniorId;
        }

        public Junior(int juniorId, string? name, int id, Wishlist wishlist) : base(name, id, wishlist)
        {
            JuniorId = juniorId;
        }
    }
}
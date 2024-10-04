using Hackathon;

namespace Hackathon.Test;

public class SimpleWishListGenerator : IWishListGenerator
{
    public List<Employee> CreateWishlist<T>(List<T> participants)
    {
        return new List<Employee>((IEnumerable<Employee>)participants);
    }
}
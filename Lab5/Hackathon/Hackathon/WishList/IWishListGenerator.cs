namespace Hackathon;

public interface IWishListGenerator
{
    public List<Employee> CreateWishlist<T>(List<T> participants);
}
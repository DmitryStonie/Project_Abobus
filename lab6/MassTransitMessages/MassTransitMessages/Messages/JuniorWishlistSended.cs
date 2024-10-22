using Hackathon;

namespace JuniorsWebApp.MassTransit;

public record JuniorWishlistSended
{
    public int HackathonId { get; init; }
    public List<Employee> Wishlist { get; init; }
    public int JuniorId { get; init; }
    public string Name { get; init; }
}
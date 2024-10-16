using Hackathon;

namespace JuniorsWebApp.MassTransit;

public record TeamleadWishlistSended
{
    public int HackathonId { get; init; }
    public List<Employee> Wishlist { get; init; }
    public int TeamleadId { get; init; }
    public string Name { get; init; }
}
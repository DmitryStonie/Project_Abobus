namespace Hackathon;

public class Wish(int score, int wishlistId, int ownerId, int partnerId)
{
    public int Id { get; set; }
    public int Score { get; init; } = score;
    public int WishlistId { get; init; } = wishlistId;
    public int OwnerId { get; init; } = ownerId;
    public int PartnerId { get; init; } = partnerId;
}
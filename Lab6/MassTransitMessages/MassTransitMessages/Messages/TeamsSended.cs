using Hackathon;

namespace MassTransitMessages.Messages;

public record TeamsSended
{
    public int HackathonId { get; init; }
    public List<Team> Teams { get; init; }
}
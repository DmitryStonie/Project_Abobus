using Hackathon;

namespace JuniorsWebApp.MassTransit;

public record TeamsSended
{
    public int HackathonId { get; init; }
    public List<Team> Teams { get; init; }
}
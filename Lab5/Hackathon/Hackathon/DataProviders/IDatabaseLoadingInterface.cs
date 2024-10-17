namespace Hackathon.DataProviders;

public interface IDatabaseLoadingInterface
{
    public HackathonDto? LoadHackathon(int id);

    public double? LoadArithmeticMean();
}
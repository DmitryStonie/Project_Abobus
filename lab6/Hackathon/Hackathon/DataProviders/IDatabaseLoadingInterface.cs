namespace Hackathon.DataProviders;

public interface IDatabaseLoadingInterface
{
    public bool LoadHackathon(int id, out List<Employee> participants, out List<Team> teams,
        out double harmonicMean);

    public double? LoadArithmeticMean();
}
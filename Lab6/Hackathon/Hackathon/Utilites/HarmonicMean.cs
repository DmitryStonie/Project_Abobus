namespace Hackathon;

public static class HarmonicMean
{
    public static double CountHarmonicMean(this List<int> numbers)
    {
        double numerator = numbers.Count;
        double denominator = 0.0;
        foreach (var number in numbers)
        {
            if (number == 0) throw new DivideByZeroException("Some of numbers is zero");
            denominator += 1.0 / number;
        }

        if (denominator == 0) throw new DivideByZeroException("Some of numbers is zero");
        return numerator / denominator;
    }
}
namespace EventLogGenerator.Services;

public static class RandomService
{
    private static readonly int _seed = 4269123;
    
    private static Random _randomGenerator = new(_seed);

    public static double GetNextDouble()
    {
        return _randomGenerator.NextDouble();
    }

    public static int GetNext(int max)
    {
        return _randomGenerator.Next(max);
    }
}
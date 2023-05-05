namespace EventLogGenerator.Services;

public static class RandomService
{
    private static int seed = 42;
    
    private static Random _randomGenerator = new(42);

    public static double GetNextDouble()
    {
        return _randomGenerator.NextDouble();
    }

    public static int GetNext(int max)
    {
        return _randomGenerator.Next(max);
    }
}
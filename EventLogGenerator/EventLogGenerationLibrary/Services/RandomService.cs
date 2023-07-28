namespace EventLogGenerator.Services;

internal static class RandomService
{
    private static readonly int _seed = 4269123;
    
    private static Random _randomGenerator = new(_seed);

    internal static double GetNextDouble()
    {
        return _randomGenerator.NextDouble();
    }

    internal static int GetNext(int max)
    {
        return _randomGenerator.Next(max);
    }
}
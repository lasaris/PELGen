namespace EventLogGenerator.Services;

/// <summary>
/// Handles randomness. If desirable, it can be specified by random or predefined seed.
/// </summary>
public static class RandomService
{
    private static int _seed = 4269123;
    
    private static Random _randomGenerator = new(_seed);

    internal static double GetNextDouble()
    {
        return _randomGenerator.NextDouble();
    }

    internal static int GetNext(int max)
    {
        return _randomGenerator.Next(max);
    }

    public static void SetRandomSeed()
    {
        _seed = new Random().Next();
        _randomGenerator = new(_seed);
    }

    public static void SetSeed(int newSeed)
    {
        _seed = newSeed;
        _randomGenerator = new(_seed);
    }
}
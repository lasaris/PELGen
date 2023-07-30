namespace EventLogGenerator.Services;

/// <summary>
/// Handles randomness. If desirable, it can be specified by random or predefined seed.
/// </summary>
internal static class RandomService
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

    internal static void SetRandomSeed()
    {
        _seed = new Random().Next();
    }

    internal static void SetSeed(int newSeed)
    {
        _seed = newSeed;
    }
}
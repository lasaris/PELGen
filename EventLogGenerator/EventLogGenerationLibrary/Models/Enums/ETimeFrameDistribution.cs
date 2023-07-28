namespace EventLogGenerator.Models;

/// <summary>
/// Defines, in which part of the timeframe is an Actvity more likely to occur
/// </summary>
internal enum ETimeFrameDistribution
{
    // Every time equal chances
    Uniform,
    // Later times linearly more likely
    Linear,
    // Later times exponentially more likely
    Exponential,
    // Earlier times linearly more likely
    ReverseLinear,
    // Earlier times exponentially more likely
    ReverseExponential,
}
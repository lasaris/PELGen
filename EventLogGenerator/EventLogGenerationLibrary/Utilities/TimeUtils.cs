using EventLogGenerator.Models;
using EventLogGenerator.Services;

namespace EventLogGenerationLibrary.Utilities;

/// <summary>
/// Utility class for additional operations regarding times and TimeFrames
/// </summary>
internal static class TimeUtils
{
    /// <summary>
    /// Randomly picks time in the interval specified by its parameters.
    /// </summary>
    /// <param name="start">Starting DateTime</param>
    /// <param name="end">Ending DateTime</param>
    /// <returns>Randomly chosen DateTime between the start and the end</returns>
    internal static DateTime PickDateInInterval(DateTime start, DateTime end)
    {
        if (start >= end)
        {
            throw new ArgumentException("Start of interval cannot be end or exceed it");
        }

        TimeSpan timeDifference = end - start;
        long randomTicks = (long)(RandomService.GetNextDouble() * timeDifference.Ticks);
        
        return start + new TimeSpan(randomTicks);
    }

    /// <summary>
    /// Randomly picks time from the List of TimeFrames
    /// </summary>
    /// <param name="timeFrames">List of possible TimeFrames to choose from</param>
    /// <returns>Randomly selected DateTime from the provided TimeFrames</returns>
    internal static DateTime PickDateFromTimeframes(List<TimeFrame> timeFrames)
    {
        List<DateTime> randomTimes = timeFrames.Select(frame => PickDateInInterval(frame.Start, frame.End)).ToList();
        
        int randomIndex = RandomService.GetNext(timeFrames.Count);
        return randomTimes[randomIndex];
    }
}    
using EventLogGenerator.Models;
using EventLogGenerator.Services;

namespace EventLogGenerator.Utilities;

public static class TimeUtils
{
    public static DateTime PickDateInInterval(DateTime start, DateTime end)
    {
        if (start >= end)
        {
            throw new ArgumentException("Start of interval cannot be end or exceed it");
        }

        TimeSpan timeDifference = end - start;
        long randomTicks = (long)(RandomService.GetNextDouble() * timeDifference.Ticks);
        
        return start + new TimeSpan(randomTicks);
    }

    public static DateTime PickDateFromTimeframes(List<TimeFrame> timeFrames)
    {
        List<DateTime> randomTimes = timeFrames.Select(frame => PickDateInInterval(frame.Start, frame.End)).ToList();
        
        int randomIndex = RandomService.GetNext(timeFrames.Count);
        return randomTimes[randomIndex];
    }
}    
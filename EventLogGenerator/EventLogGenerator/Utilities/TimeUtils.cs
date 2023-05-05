﻿using EventLogGenerator.Models;

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
        long randomTicks = (long)(new Random().NextDouble() * timeDifference.Ticks);
        
        return start + new TimeSpan(randomTicks);
    }

    public static DateTime PickDateFromTimeframes(List<TimeFrame> timeFrames)
    {
        // TODO: Create RandomService with fixed seed
        Random rand = new Random();

        List<DateTime> randomTimes = timeFrames.Select(frame => PickDateInInterval(frame.Start, frame.End)).ToList();
        
        int randomIndex = rand.Next(timeFrames.Count);
        return randomTimes[randomIndex];
    }
}    
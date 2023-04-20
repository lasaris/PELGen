namespace EventLogGenerator.Utilities;

public static class TimeUtils
{
    public static DateTime PickDateInInterval(DateTime start, DateTime end)
    {
        if (start >= end)
        {
            throw new ArgumentException("Start of ingterval cannot be end or exceed it");
        }

        TimeSpan timeDifference = end - start;
        long randomTicks = (long)(new Random().NextDouble() * timeDifference.Ticks);
        
        return start + new TimeSpan(randomTicks);
    }
}
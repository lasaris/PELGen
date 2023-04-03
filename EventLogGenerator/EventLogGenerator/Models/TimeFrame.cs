namespace EventLogGenerator.Models;

public class TimeFrame
{
    public DateTime Start;

    public DateTime End;

    public ETimeFrameDistribution Distribution;

    public TimeFrame(DateTime start, DateTime end, ETimeFrameDistribution distribution = ETimeFrameDistribution.Uniform)
    {
        if (start >= end)
        {
            throw new ArgumentException("The end of TimeFrame must be chronologically later than the start");
        }
        
        Start = start;
        End = end;
        Distribution = distribution;
    }
}
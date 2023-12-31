﻿using EventLogGenerator.Services;

namespace EventLogGenerator.Models;

/// <summary>
/// A defined time interval with Start and End DateTime attributes.
/// The Distribution attribute is used for randomly selecting a DateTime from the timeframe.
/// Optionally the attribute ExcludedTimes serves as sub-intervals that cannot be picked from the timeframe.
/// </summary>
public class TimeFrame
{
    public DateTime Start;

    public DateTime End;

    public ETimeFrameDistribution Distribution;

    public HashSet<TimeFrame>? ExcludedTimes;

    public TimeFrame(DateTime start, DateTime end, ETimeFrameDistribution distribution = ETimeFrameDistribution.Uniform,
        HashSet<TimeFrame>? excludedTimes = null)
    {
        if (start >= end)
        {
            throw new ArgumentException("The end of TimeFrame must be chronologically later than the start");
        }

        Start = start;
        End = end;
        Distribution = distribution;
        ExcludedTimes = excludedTimes;
    }

    public TimeFrame((int, int, int) start, (int, int, int) end,
        ETimeFrameDistribution distribution = ETimeFrameDistribution.Uniform, HashSet<TimeFrame>? excludedTimes = null)
        : this(new DateTime(start.Item1, start.Item2, start.Item3), new DateTime(end.Item1, end.Item2, end.Item3),
            distribution)
    {
    }

    public TimeFrame((int, int, int, int, int, int) start, (int, int, int) end,
        ETimeFrameDistribution distribution = ETimeFrameDistribution.Uniform, HashSet<TimeFrame>? excludedTimes = null)
        : this(new DateTime(start.Item1, start.Item2, start.Item3, start.Item4, start.Item5, start.Item6),
            new DateTime(end.Item1, end.Item2, end.Item3), distribution)
    {
    }

    public TimeFrame((int, int, int) start, (int, int, int, int, int, int) end,
        ETimeFrameDistribution distribution = ETimeFrameDistribution.Uniform, HashSet<TimeFrame>? excludedTimes = null)
        : this(new DateTime(start.Item1, start.Item2, start.Item3),
            new DateTime(end.Item1, end.Item2, end.Item3, end.Item4, end.Item5, end.Item6),
            distribution)
    {
    }

    public TimeFrame((int, int, int, int, int, int) start, (int, int, int, int, int, int) end,
        ETimeFrameDistribution distribution = ETimeFrameDistribution.Uniform, HashSet<TimeFrame>? excludedTimes = null)
        : this(new DateTime(start.Item1, start.Item2, start.Item3, start.Item4, start.Item5, start.Item6),
            new DateTime(end.Item1, end.Item2, end.Item3, end.Item4, end.Item5, end.Item6),
            distribution)
    {
    }

    private double WeightFunctionLinear(long ticks, long range)
    {
        return (double)(ticks - Start.Ticks) / range;
    }

    private double WeightFunctionExponential(long ticks, long range, double exponent = 2.0f)
    {
        return Math.Pow(WeightFunctionLinear(ticks, range), exponent);
    }

    public DateTime PickTimeByDistribution(DateTime? newStartLimit = null)
    {
        if (newStartLimit != null && newStartLimit >= End)
        {
            throw new ArgumentException("Cannot have limit of start after the end of current time");
        }

        DateTime pickedDateTime;

        // NOTE: This is just ugly, but "Start" is used in Weighted functions later so it is necessary (at least before refactoring)
        DateTime oldStart = new DateTime(Start.Ticks);
        // Pick new Start if old is earlier than newStartLimit
        Start = (newStartLimit != null && newStartLimit > Start) ? (DateTime)newStartLimit : Start;
        long range = (End - Start).Ticks;
        long randomTicks = (Start + TimeSpan.FromTicks((long)(RandomService.GetNextDouble() * range))).Ticks;
        switch (Distribution)
        {
            case ETimeFrameDistribution.Uniform:
                long ticks = (long)(RandomService.GetNextDouble() * range);
                pickedDateTime = Start + TimeSpan.FromTicks(ticks);
                break;

            case ETimeFrameDistribution.Linear:
                long weightedTicksLinear =
                    (long)(RandomService.GetNextDouble() * range * WeightFunctionLinear(randomTicks, range));
                pickedDateTime = Start + TimeSpan.FromTicks(weightedTicksLinear);
                break;

            case ETimeFrameDistribution.ReverseLinear:
                long weightedTicksReverseLinear = (long)(RandomService.GetNextDouble() * range *
                                                         (1 - WeightFunctionLinear(randomTicks, range)));
                pickedDateTime = Start + TimeSpan.FromTicks(weightedTicksReverseLinear);
                break;

            case ETimeFrameDistribution.Exponential:
                long weightedTicksExponential = (long)(RandomService.GetNextDouble() * range *
                                                       WeightFunctionExponential(randomTicks, range));
                pickedDateTime = Start + TimeSpan.FromTicks(weightedTicksExponential);
                break;

            case ETimeFrameDistribution.ReverseExponential:
                long weightetTicksReverseExponential = (long)(RandomService.GetNextDouble() * range *
                                                              WeightFunctionExponential(randomTicks, range, 0.5f));
                pickedDateTime = Start + TimeSpan.FromTicks(weightetTicksReverseExponential);
                break;

            default:
                throw new ArgumentException("Unknown time distribution");
        }

        // Handle invalid picked date
        if (pickedDateTime < Start || pickedDateTime > End)
        {
            throw new Exception($"Generated wrong time. Start: {Start}; End: {End}; Generated: {pickedDateTime}");
        }

        if (ExcludedTimes != null)
        {
            foreach (var time in ExcludedTimes)
            {
                if (pickedDateTime >= time.Start && pickedDateTime <= time.End)
                {
                    // NOTE: this recursive call could be a potential performance issue
                    return PickTimeByDistribution(newStartLimit);
                }
            }
        }

        Start = oldStart;
        return pickedDateTime;
    }

    public TimeFrame GetTimeFrameWithOffset(TimeSpan? startOffset = null, TimeSpan? endOffset = null)
    {
        var newStart = startOffset == null ? Start : Start + (TimeSpan)startOffset;
        var newEnd = endOffset == null ? End : End + (TimeSpan)endOffset;

        return new TimeFrame(newStart, newEnd);
    }
}
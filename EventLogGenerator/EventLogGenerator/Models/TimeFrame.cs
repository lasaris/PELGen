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

    public DateTime PickTimeByDistribution(DateTime? newStartLimit = null)
    {
        if (newStartLimit != null && newStartLimit >= End)
        {
            throw new ArgumentException("Cannot have limit of start after the end of current time");
        }

        DateTime pickedDateTime;
        Random random = new Random();
        long range = (End - Start).Ticks;
        long randomTicks = (Start + TimeSpan.FromTicks((long)(random.NextDouble() * range))).Ticks;
        switch (Distribution)
        {
            case ETimeFrameDistribution.Uniform:
                long ticks = (long)(random.NextDouble() * range);
                pickedDateTime = Start + TimeSpan.FromTicks(ticks);
                break;
            
            case ETimeFrameDistribution.Linear:
                long weightedTicksLinear = (long)(random.NextDouble() * range * WeightFunctionLinear(randomTicks, range));
                pickedDateTime = Start + TimeSpan.FromTicks(weightedTicksLinear);
                break;
            
            case ETimeFrameDistribution.ReverseLinear:
                long weightedTicksReverseLinear = (long)(random.NextDouble() * range * WeightFunctionReverseLinear(randomTicks, range));
                pickedDateTime = Start + TimeSpan.FromTicks(weightedTicksReverseLinear);
                break;
            
            case ETimeFrameDistribution.Exponential:
                long weightedTicksExponential = (long)(random.NextDouble() * range * WeightFunctionExponential(randomTicks, range));
                pickedDateTime =  Start + TimeSpan.FromTicks(weightedTicksExponential);
                break;

            case ETimeFrameDistribution.ReverseExponential:
                long weightetTicksReverseExponential = (long)(random.NextDouble() * range * WeightFunctionReverseExponential(randomTicks, range));
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

        // FIXME: Maybe handle the picked DateTime differently? Instead of adding 1, just pick randomly by given strategy in interval <startLimit, picked>?
        // The 1 in MathMax means that 2 states will differ with at least 1 tick
        return (newStartLimit == null) ? pickedDateTime : new DateTime(Math.Max(newStartLimit.Value.Ticks + TimeSpan.FromSeconds(1).Ticks, pickedDateTime.Ticks));
    }

    private double WeightFunctionLinear(long ticks, long range)
    {
        return (double)(ticks - Start.Ticks) / range;
    }

    private double WeightFunctionReverseLinear(long ticks, long range)
    {
        return 1 - (double)(ticks - Start.Ticks) / range;
    }

    private double WeightFunctionExponential(long ticks, long range, double exponent = 2.0f)
    {
        return Math.Pow((double)(ticks - Start.Ticks) / range, exponent);
    }

    private double WeightFunctionReverseExponential(long ticks, long range, double exponent = 0.5f)
    {
        return Math.Pow((double)(ticks - Start.Ticks) / range, exponent);
    }
}
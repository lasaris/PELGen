using EventLogGenerator.Models.Enums;
using EventLogGenerator.Services;

namespace EventLogGenerator.Models.States;

public class IntervalSprinkleState : ABaseState
{
    public TimeFrame TimeInterval;

    public ETimeFrameDistribution Distribution;

    public IntervalSprinkleState(EActivityType activityType, Resource resource, TimeFrame timeInterval,
        ETimeFrameDistribution distribution = ETimeFrameDistribution.Uniform) : base(activityType, resource)
    {
        TimeInterval = timeInterval;
        Distribution = distribution;

        SprinkleService.LoadIntervalSprinkleState(this);
    }
}
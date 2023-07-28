using EventLogGenerationLibrary.Services;
using EventLogGenerator.Models;
using EventLogGenerator.Services;

namespace EventLogGenerationLibrary.Models.States;

// FIXME: Fix in SprinkleService and add the option to define how many times it can be visited in that TimeInterval.
/// <summary>
/// Sprinkle that is added in a specific time interval, while the timestamp is picked by the defined Distribution.
/// </summary>
internal class IntervalSprinkleState : ABaseState
{
    internal TimeFrame TimeInterval;

    internal ETimeFrameDistribution Distribution;

    internal IntervalSprinkleState(string activityType, string resource, TimeFrame timeInterval,
        ETimeFrameDistribution distribution = ETimeFrameDistribution.Uniform) : base(activityType, resource)
    {
        TimeInterval = timeInterval;
        Distribution = distribution;

        SprinkleService.LoadIntervalSprinkleState(this);
    }
}
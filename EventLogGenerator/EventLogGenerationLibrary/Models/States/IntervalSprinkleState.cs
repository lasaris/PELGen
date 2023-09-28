using EventLogGenerationLibrary.Models.Modifiers;
using EventLogGenerationLibrary.Services;
using EventLogGenerator.Models;
using EventLogGenerator.Services;

namespace EventLogGenerationLibrary.Models.States;

/// <summary>
/// Sprinkle that is added in a specific time interval, while the timestamp is picked by the defined Distribution.
/// </summary>
public class IntervalSprinkleState : ABaseState
{
    public TimeFrame TimeInterval;

    public ETimeFrameDistribution Distribution;

    public AdditionalRandomIdModifier? Modifier;

    public IntervalSprinkleState(string activityType, string resource, TimeFrame timeInterval,
        ETimeFrameDistribution distribution = ETimeFrameDistribution.Uniform, AdditionalRandomIdModifier? modifier = null) : base(activityType, resource)
    {
        TimeInterval = timeInterval;
        Distribution = distribution;
        Modifier = modifier;
        
        SprinkleService.LoadIntervalSprinkleState(this);
    }
}
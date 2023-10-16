using EventLogGenerationLibrary.Services;
using EventLogGenerator.Models;
using EventLogGenerator.Services;

namespace EventLogGenerationLibrary.Models.States;

/// <summary>
/// Sprinkle that is dynamically added based on passed process states.
/// BeginAfter set is stores the states, that this sprinkle should be added after (or before).
/// The timestamp for this sprinkle is selected as a range from the timestamp of the process it reacts to and
/// the MaxOffset value added to it, which can also be negative. Time is then chosen based on the TimeDistribution.
/// The TimeDistribution is always applied chronologically, even when MaxOffset is negative. 
/// </summary>
public class DynamicSprinkleState : ABaseState
{
    // States after which this sprinkle can be triggered
    public HashSet<ProcessState> BeginAfter;

    // The max time after BeginAfter state that this sprinkle can be added
    public TimeSpan MaxOffset;

    // Chosen distribution for adding this sprinkle
    public ETimeFrameDistribution TimeDistribution;

    public DynamicSprinkleState(string activityType, string resource, HashSet<ProcessState> beginAfter,
        TimeSpan maxOffset, ETimeFrameDistribution timeDistribution = ETimeFrameDistribution.Uniform) : base(activityType, resource)
    {
        if (!beginAfter.Any())
        {
            throw new ArgumentException("Dynamic sprinkle must have at least one begin after state");
        }
        
        BeginAfter = beginAfter;
        MaxOffset = maxOffset;
        TimeDistribution = timeDistribution;
        
        SprinkleService.LoadDynamicSrpinkleState(this);
    }
}
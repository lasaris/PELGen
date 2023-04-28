using EventLogGenerator.Models.Enums;

namespace EventLogGenerator.Models;

public class DynamicSprinkleState : ABaseState
{
    // States after which this sprinkle can be triggered
    public HashSet<ProcessState> BeginAfter;

    // The max time after BeginAfter state that this sprinkle can be added
    public TimeSpan EndLimit;

    // Chosen distribution for adding this sprinkle
    public ETimeFrameDistribution TimeDistribution;

    protected DynamicSprinkleState(EActivityType activityType, Resource resource, HashSet<ProcessState> beginAfter,
        TimeSpan endLimit, ETimeFrameDistribution timeDistribution) : base(activityType, resource)
    {
        if (!beginAfter.Any())
        {
            throw new ArgumentException("Dynamic sprinkle must have at least one begin after state");
        }
        
        BeginAfter = beginAfter;
        EndLimit = endLimit;
        TimeDistribution = timeDistribution;
    }
}
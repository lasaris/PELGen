using EventLogGenerator.Models.Enums;
using EventLogGenerator.Services;

namespace EventLogGenerator.Models;

public class SprinkleState
{
    // Activity to be performed
    public EActivityType ActivityType;

    // Resource to be performed with
    public Resource Resource;

    // State after which sprinkle can be performed
    public ProcessState BeginAfter;

    // State after which sprinkle cannot be performed
    public ProcessState StopBefore;

    // How much likely is the sprinkle going to be used right after the BeginAfter state
    public Dictionary<ProcessState, float>? AfterStateChances;

    // How many passes for this state remain
    public int RemainingPasses;

    public SprinkleState(EActivityType activityType, Resource resource, ProcessState beginAfter,
        ProcessState stopBefore, Dictionary<ProcessState, float>? afterStateChances = null, int remainingPasses = 1,
        bool register = true)
    {
        // Cannot sparkle between same state
        if (beginAfter == stopBefore)
        {
            throw new ArgumentException("Cannot create Sprinkle with same beginning and ending state");
        }

        // Passes must be > 0
        if (remainingPasses <= 0)
        {
            throw new ArgumentException("Sprinkle must be created with at least 1 pass remaining");
        }
        
        // Cannot sprinkle after finishing state
        if (beginAfter.IsFinishing)
        {
            throw new ArgumentException("Sprinkle cannot be added after a finishing state (for now)");
        }

        ActivityType = activityType;
        Resource = resource;
        BeginAfter = beginAfter;
        StopBefore = stopBefore;
        AfterStateChances = afterStateChances;
        RemainingPasses = remainingPasses;

        if (register)
        {
            SprinkleService.LoadSprinklerState(this);
        }
    }

    // For special internal use only
    public SprinkleState(EActivityType activityType)
    {
        ActivityType = activityType;
    }
}
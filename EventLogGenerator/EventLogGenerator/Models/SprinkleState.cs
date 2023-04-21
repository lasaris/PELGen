using EventLogGenerator.Models.Enums;
using EventLogGenerator.Services;

namespace EventLogGenerator.Models;

public class SprinkleState : ABaseState
{
    // Activity to be performed
    public EActivityType ActivityType;

    // Resource to be performed with
    public Resource Resource;

    // State after which sprinkle can be performed
    public HashSet<ProcessState> BeginAfter;

    // State after which sprinkle cannot be performed
    public HashSet<ProcessState> StopBefore;

    // How much likely is the sprinkle going to be used right after the BeginAfter state
    public Dictionary<ProcessState, float>? AfterStateChances;

    // Max number of passes
    public int MaxPasses;
    
    // How many passes for this state remain
    public int RemainingPasses;

    public SprinkleState(EActivityType activityType, Resource resource, HashSet<ProcessState> beginAfter,
        HashSet<ProcessState> stopBefore, Dictionary<ProcessState, float>? afterStateChances = null, int maxPasses = 1,
        bool register = true)
    {
        // Cannot sparkle between same state
        if (beginAfter.Intersect(stopBefore).Any())
        {
            throw new ArgumentException("Cannot create Sprinkle with same beginning and ending state");
        }

        // Passes must be > 0
        if (maxPasses <= 0)
        {
            throw new ArgumentException("Sprinkle must be created with at least 1 pass remaining");
        }
        
        // Cannot sprinkle after finishing state
        if (beginAfter.Any(state => state.IsFinishing))
        {
            throw new ArgumentException("Sprinkle cannot be added after a finishing state (for now)");
        }

        ActivityType = activityType;
        Resource = resource;
        BeginAfter = beginAfter;
        StopBefore = stopBefore;
        AfterStateChances = afterStateChances;
        MaxPasses = maxPasses;
        // Set remaining passes to the maximum value at beginning
        RemainingPasses = maxPasses;

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
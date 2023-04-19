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
    public float ChanceRightAfter;

    // How many passes for this state remain
    public int RemainingPasses;

    public SprinkleState(ProcessState beginAfter, ProcessState stopBefore, EActivityType activityType,
        Resource resource, float chanceRightAfter = 1f, int remainingPasses = 1, bool register = true)
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
        
        BeginAfter = beginAfter;
        StopBefore = stopBefore;
        ActivityType = activityType;
        Resource = resource;
        ChanceRightAfter = chanceRightAfter;
        RemainingPasses = remainingPasses;

        if (register)
        {
            SprinkleService.LoadSprinklerState(this);
        }
    }
}
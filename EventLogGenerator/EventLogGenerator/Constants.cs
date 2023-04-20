namespace EventLogGenerator.GenerationLogic;

public static class Constants
{
    // Declare chances
    
    public const float ChanceToVisit = 0.5f;
    
    public const float DefaultLoopChance = 0.2f;

    public const float DeaultFinishChance = 0f;

    public const float WaitForNewStateChance = 0.95f;
    
    // Process evaluation points weights for next state jump (ideally in range 0 - 1000)

    public const float ChanceToFollowWeight = 500;
    
    public const float CompulsoryWeight = 100;

    public const float DifferentActivityWeight = 5;

    public const float EachPreviousVisitWeight = -20;

    public const float LastVisitWeight = 0;

    public const float SameResourceWeight = 0;

    public const float ToFinishingWeight = 20;
    
    // Constants for services

    public const float NoSprinkleChance = 0.6f;

    public const float SprinkleAfterStateWeight = 100;
}
namespace EventLogGenerator.GenerationLogic;

public static class Constants
{
    public const float ChanceToVisit = 0.5f;
    
    public const float DefaultLoopChance = 0.2f;

    public const float DeaultFinishChance = 0f;
    
    // Process evaluation points weights for next state jump (ideally in range 0 - 1000)

    public const float CompulsoryWeight = 30;

    public const float ChanceToFollowWeight = 25;

    public const float ChanceToVisitWeight = 20;

    public const float LoopChanceWeight = 5;

    public const float DifferentActivityWeight = 5;

    public const float EachPreviousVisitWeight = -5;

    public const float LastVisitWeight = 0;

    public const float SameResourceWeight = 5;

    public const float ToFinishingWeight = 15;
}
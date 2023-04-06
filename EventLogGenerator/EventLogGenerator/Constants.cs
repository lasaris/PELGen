namespace EventLogGenerator.GenerationLogic;

public static class Constants
{
    public const float ChanceToVisit = 0.5f;
    
    public const float DefaultLoopChance = 0.2f;

    public const float DeaultFinishChance = 0f;
    
    // Process evaluation points weights for next state jump (ideally in range 0 - 1000)

    public const float CompulsoryWeight = 300;

    public const float ChanceToFollowWeight = 200;

    public const float ChanceToVisitWeight = 200;

    public const float LoopChanceWeight = 200;

    public const float DifferentActivityWeight = 50;

    public const float EachPreviousVisitWeight = -50;

    public const float LastVisitWeight = -10;

    public const float SameResourceWeight = 25;

    public const float ToFinishingWeight = 100;
}
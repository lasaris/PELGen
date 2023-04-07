﻿namespace EventLogGenerator.GenerationLogic;

public static class Constants
{
    // Declare chances
    
    public const float ChanceToVisit = 0.5f;
    
    public const float DefaultLoopChance = 0.2f;

    public const float DeaultFinishChance = 0f;

    public const float WaitForNewStateChance = 0.95f;
    
    // Process evaluation points weights for next state jump (ideally in range 0 - 1000)

    public const float CompulsoryWeight = 50;

    public const float ChanceToFollowWeight = 40;

    public const float ChanceToVisitWeight = 20;

    public const float LoopChanceWeight = 5;

    public const float DifferentActivityWeight = 5;

    public const float EachPreviousVisitWeight = 0;

    public const float LastVisitWeight = 0;

    public const float SameResourceWeight = 0;

    public const float ToFinishingWeight = 20;
}
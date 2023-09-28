using EventLogGenerationLibrary.GenerationLogic;
using EventLogGenerationLibrary.Models;
using EventLogGenerationLibrary.Models.Events;
using EventLogGenerationLibrary.Models.States;
using EventLogGenerationLibrary.Utilities;
using EventLogGenerator.Models;
using EventLogGenerator.Services;

namespace EventLogGenerationLibrary.Services;

/// <summary>
/// Used to collect all sprinkles and then use them to "sprinkle" process with them. 
/// </summary>
internal static class SprinkleService
{
    // Delegate for handling event of entering state
    internal delegate void StateEnteredHandler(object sender, StateEnteredArgs data);

    // Define event for state entering that uses the delegate above
    internal static event StateEnteredHandler SprinkleAdded;

    // Sprinkles currently ready to be sprinkled into the process
    internal static HashSet<SprinkleState> Sprinkles = new();

    // Dynamic srinkles currently ready to be sprinkled into the process
    internal static HashSet<DynamicSprinkleState> DynamicSprinkles = new();

    // Interval sprinkles with time distribution
    internal static HashSet<IntervalSprinkleState> IntervalSprinkles = new();

    // Maps sprinkles to available timeframes for given actorframe
    internal static Dictionary<SprinkleState, List<TimeFrame>> SprinkleTimeMap = new();

    // Keeps track of sprinkles that were added by the service
    internal static List<(ABaseState, DateTime)> SprinkleStack = new();

    // Track periodic sprinkles
    internal static HashSet<PeriodicSprinkleState> PeriodicSprinkles = new();

    // Track conditional sprinkles
    internal static HashSet<ConditionalSprinkle> ConditionalSprinkles = new();

    private static void OnSprinkleAdd(ABaseState sprinkle, Actor actor, DateTime timeStamp, string? additional = null)
    {
        var newEvent = new StateEnteredArgs(sprinkle, actor, timeStamp, additional);
        SprinkleStack.Add((sprinkle, timeStamp));
        SprinkleAdded.Invoke(null, newEvent);
        // FIXME: Should the logging be done by FileManager instead?
        Console.Out.WriteLine($"[INFO] {actor.Id} Added Sprinkle {sprinkle.ActivityType} - {sprinkle.Resource}");
    }

    private static void AddSprinkle(SprinkleState sprinkle, Actor actor)
    {
        var sprinkleTime = TimeUtils.PickDateFromTimeframes(SprinkleTimeMap[sprinkle]);
        OnSprinkleAdd(sprinkle, actor, sprinkleTime);
    }

    private static void AddDynamicSprinkle(DynamicSprinkleState dynamicSprinkle, Actor actor, DateTime start)
    {
        TimeFrame dynamicTimeFrame = dynamicSprinkle.MaxOffset.Ticks > 0
            ? new TimeFrame(start, start + dynamicSprinkle.MaxOffset, dynamicSprinkle.TimeDistribution)
            : new TimeFrame(start + dynamicSprinkle.MaxOffset, start, dynamicSprinkle.TimeDistribution);
        var sprinkleTime = dynamicTimeFrame.PickTimeByDistribution();
        OnSprinkleAdd(dynamicSprinkle, actor, sprinkleTime);
    }

    private static void AddIntervalSprinkle(IntervalSprinkleState sprinkle, Actor actor, string? additional = null)
    {
        var pickedTime = sprinkle.TimeInterval.PickTimeByDistribution();
        if (additional == null)
        {
            OnSprinkleAdd(sprinkle, actor, pickedTime);
        }
        else
        {
            OnSprinkleAdd(sprinkle, actor, pickedTime, additional);
        }
    }

    private static void AddPeriodSprinkle(PeriodicSprinkleState sprinkle, Actor actor, DateTime timestamp)
    {
        OnSprinkleAdd(sprinkle, actor, timestamp);
    }

    internal static void LoadSprinkleState(SprinkleState sprinkle)
    {
        Sprinkles.Add(sprinkle);
    }

    internal static void LoadDynamicSrpinkleState(DynamicSprinkleState sprinkle)
    {
        DynamicSprinkles.Add(sprinkle);
    }

    internal static void LoadIntervalSprinkleState(IntervalSprinkleState sprinkle)
    {
        IntervalSprinkles.Add(sprinkle);
    }

    internal static void LoadPeriodicSprinkle(PeriodicSprinkleState sprinkle)
    {
        PeriodicSprinkles.Add(sprinkle);
    }

    internal static void LoadConditionalSprinkle(ConditionalSprinkle sprinkle)
    {
        ConditionalSprinkles.Add(sprinkle);
    }

    internal static void RunIntervalSprinkles(Actor actor)
    {
        foreach (var sprinkle in IntervalSprinkles)
        {
            string? additional = null;

            if (sprinkle.Modifier != null && sprinkle.Modifier.ActionsToReact.Contains(sprinkle.ActivityType))
            {
                for (int i = 0; i < sprinkle.Modifier.NumberOfOccurrences; i++)
                {
                    additional = sprinkle.Modifier.GetRandomActorId();
                    AddIntervalSprinkle(sprinkle, actor, additional);
                }
            }
            else
            {
                AddIntervalSprinkle(sprinkle, actor, additional);
            }
        }
    }

    // FIXME: Ideally remove the dependencies on actor frame and make it more generalized (like reactive states)
    internal static void RunSprinkling(ActorFrame filledActorFrame)
    {
        // Create possible timeframes for each sprinkle
        foreach (var sprinkle in Sprinkles)
        {
            // Reset map of sprinkle timeframes
            SprinkleTimeMap[sprinkle] = new();

            DateTime? startTime = null;
            bool isSkipped = false;
            foreach (var stateTimePair in filledActorFrame.VisitedStack)
            {
                // Handle skipping
                if (sprinkle.SkipStart != null && sprinkle.SkipEnd != null)
                {
                    if (!isSkipped && sprinkle.SkipStart.Contains(stateTimePair.Item1))
                    {
                        if (startTime != null)
                        {
                            SprinkleTimeMap[sprinkle].Add(new TimeFrame((DateTime)startTime, stateTimePair.Item2));
                            startTime = null;
                            isSkipped = true;
                        }
                    }
                    else if (isSkipped && sprinkle.SkipEnd.Contains(stateTimePair.Item1))
                    {
                        startTime = stateTimePair.Item2;
                        isSkipped = false;
                    }
                }

                // Handle beginning and ending of allowed range
                if (startTime == null && sprinkle.BeginAfter.Contains(stateTimePair.Item1))
                {
                    startTime = stateTimePair.Item2;
                }

                else if (startTime != null && sprinkle.StopBefore.Contains(stateTimePair.Item1))
                {
                    SprinkleTimeMap[sprinkle].Add(new TimeFrame((DateTime)startTime, stateTimePair.Item2));
                    startTime = null;
                }
            }
        }

        // Add each sprinkle in random time of created timeframes
        foreach (var sprinkle in Sprinkles)
        {
            if (!SprinkleTimeMap[sprinkle].Any())
            {
                // This means that currently the sprinkle cannot be applied
                continue;
            }

            for (int i = 0; i < sprinkle.Passes; i++)
            {
                AddSprinkle(sprinkle, filledActorFrame.Actor);
            }
        }

        // Execute dynamic sprinkles
        // FIXME: This could theoretically be unified with normal sprinkles which go through the visited stack
        foreach (var sprinkle in DynamicSprinkles)
        {
            foreach (var stateTimePair in filledActorFrame.VisitedStack)
            {
                if (sprinkle.BeginAfter.Contains(stateTimePair.Item1))
                {
                    AddDynamicSprinkle(sprinkle, filledActorFrame.Actor, stateTimePair.Item2);
                }
            }
        }

        // Execute periodic sprinkles
        foreach (var sprinkle in PeriodicSprinkles)
        {
            DateTime? beginTime = null;
            foreach (var stateTimePair in filledActorFrame.VisitedStack)
            {
                if (beginTime == null && sprinkle.BeginAfter.Contains(stateTimePair.Item1))
                {
                    beginTime = stateTimePair.Item2;
                }
                else if (beginTime != null && sprinkle.StopBefore.Contains(stateTimePair.Item1))
                {
                    DateTime endTime = stateTimePair.Item2;
                    List<DateTime> timeStamps = new();
                    DateTime currentTime = (DateTime)beginTime + sprinkle.Period;
                    int pickedAlternativeCount = 0;
                    // Dirty hack to compare the whole seconds
                    var periodSeconds = (int)sprinkle.Period.TotalSeconds;
                    var elapsedSeconds = (int)(endTime - currentTime).TotalSeconds;
                    var numIterations = elapsedSeconds / periodSeconds;
                    for (int i = 0; i < numIterations; i++)
                    {
                        var pickAlternative = (sprinkle.AlternativeState != null) && RandomService.GetNextDouble() < sprinkle.AlternativeState.Value.Item2;
                        if (pickAlternative && pickedAlternativeCount < sprinkle.AlternativeState.Value.Item3 && i != 0 && i != numIterations - 1)
                        {
                            OnSprinkleAdd(sprinkle.AlternativeState.Value.Item1, filledActorFrame.Actor, currentTime);
                            ++pickedAlternativeCount;
                        }
                        else
                        {
                            AddPeriodSprinkle(sprinkle, filledActorFrame.Actor, currentTime);
                        }
                        currentTime += sprinkle.Period;
                    }

                    beginTime = null;
                }
            }
        }

        // Sprinkle conditionals
        foreach (var sprinkle in ConditionalSprinkles)
        {
            if (filledActorFrame.VisitedMap.ContainsKey(sprinkle.StateToOccur))
            {
                foreach (var stateTimePair in filledActorFrame.VisitedStack)
                {
                    if (stateTimePair.Item1 == sprinkle.StateToOccur)
                    {
                        OnSprinkleAdd(sprinkle.PositiveState, filledActorFrame.Actor,
                            stateTimePair.Item2 + sprinkle.TimeOffset);
                    }
                }
            }
            else
            {
                var ourTheoreticalTime = sprinkle.StateToOccur.TimeFrame.PickTimeByDistribution();
                ourTheoreticalTime +=
                    ActorService.GetActorActivityOffset(filledActorFrame.Actor, sprinkle.StateToOccur.ActivityType);
                OnSprinkleAdd(sprinkle.NegativeState, filledActorFrame.Actor, ourTheoreticalTime + sprinkle.TimeOffset);
            }
        }

        RunIntervalSprinkles(filledActorFrame.Actor);
    }

    internal static void ResetService()
    {
        Sprinkles = new();
        DynamicSprinkles = new();
        SprinkleTimeMap = new();
        SprinkleStack = new();
        IntervalSprinkles = new();
        ConditionalSprinkles = new();
    }
}
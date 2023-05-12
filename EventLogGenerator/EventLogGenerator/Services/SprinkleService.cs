using EventLogGenerator.GenerationLogic;
using EventLogGenerator.Models;
using EventLogGenerator.Models.Enums;
using EventLogGenerator.Models.Events;
using EventLogGenerator.Models.States;
using EventLogGenerator.Utilities;

namespace EventLogGenerator.Services;

/**
 * Sprinkles process with events from given checkpoints. These checkpoints can be performed anytime after some state.
 */
public static class SprinkleService
{
    // Delegate for handling event of entering state
    public delegate void StateEnteredHandler(object sender, StateEnteredEvent data);

    // Define event for state entering that uses the delegate above
    public static event StateEnteredHandler SprinkleAdded;

    // Sprinkles currently ready to be sprinkled into the process
    public static HashSet<SprinkleState> Sprinkles = new();

    // Dynamic srinkles currently ready to be sprinkled into the process
    public static HashSet<DynamicSprinkleState> DynamicSprinkles = new();

    // Interval sprinkles with time distribution
    public static HashSet<IntervalSprinkleState> IntervalSprinkleStates = new();

    // Maps sprinkles to available timeframes for given actorframe
    public static Dictionary<SprinkleState, List<TimeFrame>> SprinkleTimeMap = new();

    // Keeps track of sprinkles that were added by the service
    public static List<(ABaseState, DateTime)> SprinkleStack = new();

    // Track dynamic sprinkle mutexes
    public static HashSet<DynamicSprinkleMutex> DynamicSprinkleMutexes = new();

    // Track periodic sprinkles
    public static HashSet<PeriodicSprinkleState> PeriodicSprinkles = new();

    // Track scenario sprinkles
    public static HashSet<ScenarioSetSprinkle> ScenarioSprinkles = new();

    // Track conditional sprinkles
    public static HashSet<ConditionalSprinkle> ConditionalSprinkles = new();

    private static void OnSprinkleAdd(ABaseState sprinkle, Actor actor, DateTime timeStamp, string? additional = null)
    {
        var newEvent = new StateEnteredEvent(sprinkle, actor, timeStamp, additional);
        SprinkleStack.Add((sprinkle, timeStamp));
        SprinkleAdded.Invoke(null, newEvent);
        // FIXME: Should the logging be done by EventLogger instead?
        Console.Out.WriteLine($"[INFO] {actor.Id} Added Sprinkle {sprinkle.ActivityType} - {sprinkle.Resource.Name}");
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

    public static void LoadSprinklerState(SprinkleState sprinkle)
    {
        Sprinkles.Add(sprinkle);
    }

    public static void LoadDynamicSrpinkleState(DynamicSprinkleState sprinkle)
    {
        DynamicSprinkles.Add(sprinkle);
    }

    public static void LoadIntervalSprinkleState(IntervalSprinkleState sprinkle)
    {
        IntervalSprinkleStates.Add(sprinkle);
    }

    public static void LoadDynamicSrpinkleMutex(DynamicSprinkleMutex dynamicSprinkleMutex)
    {
        if (!DynamicSprinkles.Contains(dynamicSprinkleMutex.FirstState)
            || !DynamicSprinkles.Contains(dynamicSprinkleMutex.SecondState))
        {
            throw new ArgumentException(
                "Dynamic sprinkle mutex must contain states that are registerd in Sprinkle service");
        }

        DynamicSprinkles.Remove(dynamicSprinkleMutex.FirstState);
        DynamicSprinkles.Remove(dynamicSprinkleMutex.SecondState);
        DynamicSprinkleMutexes.Add(dynamicSprinkleMutex);
    }


    public static void LoadPeriodicSprinkle(PeriodicSprinkleState sprinkle)
    {
        PeriodicSprinkles.Add(sprinkle);
    }

    public static void LoadScenarioSprinkle(ScenarioSetSprinkle sprinkle)
    {
        ScenarioSprinkles.Add(sprinkle);
    }

    public static void LoadConditionalSprinkle(ConditionalSprinkle sprinkle)
    {
        ConditionalSprinkles.Add(sprinkle);
    }

    public static void RunIntervalSprinkles(Actor actor)
    {
        foreach (var sprinkle in IntervalSprinkleStates)
        {
            string? additional = null;
            // FIXME: This is hardcoded and should be somehow abstracted
            if (sprinkle.ActivityType == EActivityType.VisitStudentRecord)
            {
                var totalNumberOfStudents = (int) (Collector.GetLastCollectionMaxId() + 1 - IdService.InitialSetId);
                var studentsVisited = Math.Max(RandomService.GetNext(totalNumberOfStudents / 2), totalNumberOfStudents / 6);
                for (int i = 0; i < studentsVisited; i++)
                {
                    additional = (RandomService.GetNext(totalNumberOfStudents) + IdService.InitialSetId).ToString();
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
    public static void RunSprinkling(ActorFrame filledActorFrame)
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

        // Execute dynamic sprinkle mutexes
        foreach (var mutex in DynamicSprinkleMutexes)
        {
            foreach (var stateTimePair in filledActorFrame.VisitedStack)
            {
                var sprinkle = mutex.PickState();

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

        // Sprinkle scenarios
        foreach (var scenario in ScenarioSprinkles)
        {
            // Randomly skipped based on scenario chance to occur
            if (RandomService.GetNextDouble() > scenario.ChanceToOccur)
            {
                continue;
            }

            DateTime? beginTime = null;
            foreach (var stateTimePair in filledActorFrame.VisitedStack)
            {
                if (beginTime == null && scenario.StartStates.Contains(stateTimePair.Item1))
                {
                    beginTime = stateTimePair.Item2;
                }
                else if (beginTime != null && scenario.EndStates.Contains(stateTimePair.Item1))
                {
                    DateTime currentTime = (DateTime)beginTime;
                    foreach (var stateOffsetPair in scenario.ScenarioSprinkleOffsets)
                    {
                        var minTime = currentTime + stateOffsetPair.Item2;
                        var maxTime = currentTime + stateOffsetPair.Item3;
                        currentTime = TimeUtils.PickDateInInterval(minTime, maxTime);
                        OnSprinkleAdd(stateOffsetPair.Item1, filledActorFrame.Actor, currentTime);
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

    public static void ResetService()
    {
        Sprinkles = new();
        DynamicSprinkles = new();
        SprinkleTimeMap = new();
        SprinkleStack = new();
        IntervalSprinkleStates = new();
        ScenarioSprinkles = new();
        ConditionalSprinkles = new();
    }
}
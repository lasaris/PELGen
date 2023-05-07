using EventLogGenerator.Exceptions;
using EventLogGenerator.Models;
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
    public delegate void SprinkleAddedhandler(object sender, SprinkleAddedEvent data);

    // Define event for state entering that uses the delegate above
    public static event SprinkleAddedhandler SprinkleAdded;

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

    private static void OnSprinkleAdd(ABaseState sprinkle, Actor actor, DateTime timeStamp)
    {
        var newEvent = new SprinkleAddedEvent(sprinkle, actor, timeStamp);
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

    private static void AddIntervalSprinkle(IntervalSprinkleState sprinkle, Actor actor)
    {
        var pickedTime = sprinkle.TimeInterval.PickTimeByDistribution();
        OnSprinkleAdd(sprinkle, actor, pickedTime);
    }

    public static void LoadSprinklerState(SprinkleState newSprinkle)
    {
        Sprinkles.Add(newSprinkle);
    }

    public static void LoadDynamicSrpinkleState(DynamicSprinkleState newSprinkle)
    {
        DynamicSprinkles.Add(newSprinkle);
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

    public static void RunIntervalSprinkles(Actor actor)
    {
        foreach (var sprinkle in IntervalSprinkleStates)
        {
            AddIntervalSprinkle(sprinkle, actor);
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
    }

    public static void ResetService()
    {
        Sprinkles = new();
        DynamicSprinkles = new();
        SprinkleTimeMap = new();
        SprinkleStack = new();
    }
}
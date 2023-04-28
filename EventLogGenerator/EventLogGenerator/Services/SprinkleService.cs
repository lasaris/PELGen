using System.Data;
using EventLogGenerator.Exceptions;
using EventLogGenerator.Models;
using EventLogGenerator.Models.Enums;
using EventLogGenerator.Utilities;
using Constants = EventLogGenerator.GenerationLogic.Constants;

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

    // Maps sprinkles to available timeframes for given actorframe
    public static Dictionary<SprinkleState, List<TimeFrame>> SprinkleTimeMap = new();

    private static void OnSprinkleAdd(SprinkleState sprinkle, Actor actor, DateTime timeStamp)
    {
        var newEvent = new SprinkleAddedEvent(sprinkle, actor, timeStamp);
        SprinkleAdded.Invoke(null, newEvent);
        // FIXME: Should the logging be done by EventLogger instead?
        Console.Out.WriteLine($"[INFO] {actor.Id} Added Sprinkle {sprinkle.ActivityType} - {sprinkle.Resource.Name}");
    }

    private static void AddSprinkle(SprinkleState sprinkle, Actor actor)
    {
        var sprinkleTime = TimeUtils.PickDateFromTimeframes(SprinkleTimeMap[sprinkle]);
        OnSprinkleAdd(sprinkle, actor, sprinkleTime);
    }

    public static void LoadSprinklerState(SprinkleState newSprinkle)
    {
        Sprinkles.Add(newSprinkle);
    }

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
                throw new InvalidSprinklerState("Sprinkle must have at least one timeframe created");
            }

            for (int i = 0; i < sprinkle.Passes; i++)
            {
                AddSprinkle(sprinkle, filledActorFrame.Actor);
            }
        }
    }
}
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
    
    // Sprinkles, that are yet to be sprinkled
    public static HashSet<SprinkleState> UpcomingSprinkles = new();

    // Sprinkles currently ready to be sprinkled into the process
    public static HashSet<SprinkleState> AvailableSprinkles = new();
    
    // Sprinkles, that were previously used
    public static HashSet<SprinkleState> UsedSprinkles = new();

    // Time of last event
    private static DateTime LastTimestamp;

    // If we want the process to be sprinkled or not
    public static bool Enabled = true;
    
    private static void UpdateAvailableSprinkles(ProcessState newState)
    {
        var totalSprinkles = UpcomingSprinkles.Count + AvailableSprinkles.Count + UsedSprinkles.Count; 
        // Remove unusable sprinkles
        HashSet<SprinkleState> newAvailable = new();
        foreach (var state in AvailableSprinkles)
        {
            if (state.StopBefore.Contains(newState) || state.RemainingPasses == 0)
            {
                UsedSprinkles.Add(state);
            }
            else
            {
                newAvailable.Add(state);
            }
        }

        AvailableSprinkles = newAvailable;

        // Add newly available sprinkles and remove them from upcoming
        var newlyAvailable = UpcomingSprinkles.Where(state => state.BeginAfter.Contains(newState)).ToHashSet();
        AvailableSprinkles.UnionWith(newlyAvailable);
        UpcomingSprinkles.ExceptWith(newlyAvailable);

        // Defensive mechanism for integrity control
        if (totalSprinkles != UpcomingSprinkles.Count + AvailableSprinkles.Count + UsedSprinkles.Count)
        {
            throw new InvalidSprinklerState("Some sprinkles were lost in the update process");
        }
    }

    private static void AddRandomSprinkle(ProcessState newState, Actor actor, DateTime enteredTime)
    {
        if (!AvailableSprinkles.Any())
        {
            return;
        }
        
        // Weight each state
        var weightedStates = new Dictionary<SprinkleState, float>();
        foreach (var sprinkle in AvailableSprinkles)
        {
            float weight = 1;
            
            // TODO: AfterStateChances should probably be handled separately before random selection of sprinkle? 
            // Rank afterStateChances
            if (sprinkle.AfterStateChances != null && sprinkle.AfterStateChances.ContainsKey(newState))
            {
                weight += Constants.SprinkleAfterStateWeight * sprinkle.AfterStateChances[newState];
            }
            
            weightedStates.Add(sprinkle, weight);
        }
        var newSprinkle = SelectNextSprinkle(weightedStates);
        
        // Dont sprinkle
        if (newSprinkle == null)
        {
            return;
        }

        if (LastTimestamp == null)
        {
            throw new ArgumentException("Cannot have defined sprinkles and no previous state time");
        }

        var sprinkleTime = TimeUtils.PickDateInInterval(LastTimestamp, enteredTime);
        
        OnSprinkleAdd(newSprinkle, actor, sprinkleTime);
    }

    private static SprinkleState? SelectNextSprinkle(Dictionary<SprinkleState, float> weightedSprinkles)
    {
        var random = new Random();
        // Add special no sprinkle state
        weightedSprinkles.Add(new SprinkleState(EActivityType.NoSprinkle), Constants.NoSprinkleWeight);
        
        float totalWeight = weightedSprinkles.Values.Sum();
        float randomWeight = (float)random.NextDouble() * totalWeight;
        float cumulativeWeight = 0f;

        foreach (var kvp in weightedSprinkles)
        {
            cumulativeWeight += kvp.Value;
            if (randomWeight < cumulativeWeight)
            {
                if (kvp.Key.ActivityType == EActivityType.NoSprinkle)
                {
                    return null;
                }
                
                return kvp.Key;
            }
        }
        
        throw new InvalidOperationException("Cannot pick from empty state-weight pairs");
    }

    private static void OnSprinkleAdd(SprinkleState sprinkle, Actor actor, DateTime timeStamp)
    {
        // TODO: This applies to all actors -> only first passes if remaining passes are decremented
        sprinkle.RemainingPasses -= 1;
        var newEvent = new SprinkleAddedEvent(sprinkle, actor, timeStamp);
        SprinkleAdded.Invoke(null, newEvent);
        Console.Out.WriteLine($"[INFO] {actor.Id} Added Sprinkle {sprinkle.ActivityType} - Remaining {sprinkle.RemainingPasses}");
    }

    public static void StateEnteredHandler(object sender, StateEnteredEvent data)
    {
        if (!Enabled)
        {
            return;
        }
        
        AddRandomSprinkle(data.State, data.Actor, data.TimeStamp);
        UpdateAvailableSprinkles(data.State);
        LastTimestamp = data.TimeStamp;
    }

    public static void ResetAvailableSprinkles()
    {
        UpcomingSprinkles = new HashSet<SprinkleState>(UsedSprinkles);
        foreach (var sprinkle in UpcomingSprinkles)
        {
            sprinkle.RemainingPasses = sprinkle.MaxPasses;
        }
        UsedSprinkles = new();
        AvailableSprinkles = new();
    }

    public static void ResetSprinklerState()
    {
        AvailableSprinkles = new();
        UpcomingSprinkles = new();
    }

    public static void LoadSprinklerState(SprinkleState newSprinkle)
    {
        UpcomingSprinkles.Add(newSprinkle);
    }
}
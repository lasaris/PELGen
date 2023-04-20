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
    
    // Time of last event
    private static DateTime LastTimestamp;
    
    private static void UpdateAvailableSprinkles(ProcessState newState)
    {
        // Remove unusable sprinkles
        AvailableSprinkles = AvailableSprinkles.Where(state => state.StopBefore != newState && state.RemainingPasses > 0).ToHashSet();

        // Add newly available sprinkles and remove them from upcoming
        var newlyAvailable = UpcomingSprinkles.Where(state => state.BeginAfter == newState);
        newlyAvailable.Select(state => AvailableSprinkles.Add(state));
        UpcomingSprinkles.ExceptWith(newlyAvailable.ToHashSet());
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
        LastTimestamp = enteredTime;
        
        OnSprinkleAdd(newSprinkle, actor, sprinkleTime);
    }

    private static SprinkleState? SelectNextSprinkle(Dictionary<SprinkleState, float> weightedSprinkles)
    {
        // Calculate the chance of not picking any state
        float totalChance = weightedSprinkles.Values.Sum() + Constants.NoSprinkleChance;
        float noStateChance =  Constants.NoSprinkleChance / totalChance;
        
        // Add special no sprinkle state
        weightedSprinkles.Add(new SprinkleState(EActivityType.NoSprinkle), noStateChance);
        
        Random random = new Random();
        float randomValue = (float)random.NextDouble();

        float cumulativeChance = 0;
        foreach (var entry in weightedSprinkles)
        {
            cumulativeChance += entry.Value;
            if (randomValue < cumulativeChance)
            {
                if (entry.Key.ActivityType == EActivityType.NoSprinkle)
                {
                    return null;
                }

                return entry.Key;
            }
        }
        
        throw new InvalidOperationException("Cannot pick from empty state-weight pairs");
    }

    private static void OnSprinkleAdd(SprinkleState sprinkle, Actor actor, DateTime timeStamp)
    {
        var newEvent = new SprinkleAddedEvent(sprinkle, actor, timeStamp);
        SprinkleAdded.Invoke(null, newEvent);
    }

    public static void StateEnteredHandler(object sender, StateEnteredEvent data)
    {
        UpdateAvailableSprinkles(data.State);
        AddRandomSprinkle(data.State, data.Actor, data.TimeStamp);
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
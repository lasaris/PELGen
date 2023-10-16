using EventLogGenerationLibrary.Models;

namespace EventLogGenerationLibrary.Services;

/// <summary>
/// Handles operations related to Actor.
/// </summary>
public static class ActorService
{
    // Maps each actor to a list of offsets for different activities (useful when we want certain actors to have a time offset)
    internal static Dictionary<Actor, Dictionary<string, TimeSpan>> ActorOffsetMap = new();

    public static void SetActivitiesOffset(Actor actor, HashSet<string> activities, TimeSpan offset)
    {
        foreach (var activity in activities)
        {
            SetOffset(actor, activity, offset);
        }
    }
    
    internal static void SetOffset(Actor actor, string activity, TimeSpan offset)
    {
        if (ActorOffsetMap.ContainsKey(actor))
        {
            ActorOffsetMap[actor][activity] = offset;
        }
        else
        {
            ActorOffsetMap[actor] = new Dictionary<string, TimeSpan>
            {
                [activity] = offset
            };
        }
    }

    internal static TimeSpan GetActorActivityOffset(Actor actor, string activity)
    {
        if (!ActorOffsetMap.ContainsKey(actor))
        {
            return TimeSpan.Zero;
        }
        
        if (!ActorOffsetMap[actor].ContainsKey(activity))
        {
            return TimeSpan.Zero;
        }
        
        return ActorOffsetMap[actor][activity];
    }

    internal static void ResetService()
    {
        ActorOffsetMap = new();
    }
}
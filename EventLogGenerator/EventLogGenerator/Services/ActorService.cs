using EventLogGenerator.Models;
using EventLogGenerator.Models.Enums;

namespace EventLogGenerator.Services;

public static class ActorService
{
    // Maps each actor to a list of offsets for different activities (useful when we want certain actors to have a time offset)
    public static Dictionary<Actor, Dictionary<EActivityType, TimeSpan>> ActorOffsetMap = new();

    public static void RegisterActivitiesOffset(Actor actor, HashSet<EActivityType> activities, TimeSpan offset)
    {
        foreach (var activity in activities)
        {
            RegisterOffset(actor, activity, offset);
        }
    }
    
    public static void RegisterOffset(Actor actor, EActivityType activity, TimeSpan offset)
    {
        if (ActorOffsetMap.ContainsKey(actor))
        {
            if (ActorOffsetMap[actor].ContainsKey(activity))
            {
                throw new ArgumentException("ActivityType was already registered to an offset for the actor");
            }

            ActorOffsetMap[actor][activity] = offset;
        }
        else
        {
            ActorOffsetMap[actor] = new Dictionary<EActivityType, TimeSpan>
            {
                [activity] = offset
            };
        }
    }

    public static TimeSpan GetActorActivityOffset(Actor actor, EActivityType activity)
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
}
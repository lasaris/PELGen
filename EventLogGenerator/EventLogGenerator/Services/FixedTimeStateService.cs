using EventLogGenerator.Models;
using EventLogGenerator.Models.Events;

namespace EventLogGenerator.Services;

public static class FixedTimeStateService
{
    // Delegate for handling event of entering state
    public delegate void StateEnteredHandler(object sender, StateEnteredEvent data);

    // Define event for state entering that uses the delegate above
    public static event StateEnteredHandler StateEntered;

    // Sprinkles currently ready to be sprinkled into the process
    public static HashSet<FixedTimeState> FixedTimeStates = new();

    private static void OnReactionAdd(FixedTimeState state, Actor actor, DateTime timeStamp)
    {
        var newEvent = new StateEnteredEvent(state, actor, timeStamp);
        StateEntered.Invoke(null, newEvent);
        // FIXME: Should the logging be done by EventLogger instead?
        Console.Out.WriteLine($"[INFO] {actor.Id} Added Fixed state {state.ActivityType} - {state.Resource.Name}");
    }

    private static void AddFixedState(FixedTimeState state, Actor actor)
    {
        OnReactionAdd(state, actor, state.VisitTime);
    }

    public static void RunFixedStates(Actor actor)
    {
        foreach (var state in FixedTimeStates)
        {
            AddFixedState(state, actor);
        }
    }

    public static void LoadFixedState(FixedTimeState state)
    {
        FixedTimeStates.Add(state);
    }

    public static void ResetService()
    {
        FixedTimeStates = new();
    }
}
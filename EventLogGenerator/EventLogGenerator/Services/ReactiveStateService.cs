using EventLogGenerator.Models;

namespace EventLogGenerator.Services;

public static class ReactiveStateService
{
    // Delegate for handling event of entering state
    public delegate void StateEnteredHandler(object sender, StateEnteredEvent data);

    // Define event for state entering that uses the delegate above
    public static event StateEnteredHandler StateEntered;

    // Sprinkles currently ready to be sprinkled into the process
    public static HashSet<ReactiveState> ReactiveStates = new();

    private static void OnReactionAdd(ReactiveState state, Actor actor, DateTime timeStamp)
    {
        var newEvent = new StateEnteredEvent(state, actor, timeStamp);
        StateEntered.Invoke(null, newEvent);
        // FIXME: Should the logging be done by EventLogger instead?
        Console.Out.WriteLine($"[INFO] {actor.Id} Added Reactive state {state.ActivityType} - {state.Resource.Name}");
    }

    private static void AddReactiveState(ReactiveState reactiveState, DateTime reactionTime)
    {
        OnReactionAdd(reactiveState, reactiveState.ReactingActor, reactionTime);
    }

    public static void RunReactiveStates(ActorFrame filledActorFrame)
    {
        foreach (var stateTimePair in filledActorFrame.VisitedStack)
        {
            foreach (var reactiveState in ReactiveStates)
            {
                if (stateTimePair.Item1.Equals(reactiveState.ReactTo))
                {
                    AddReactiveState(reactiveState, stateTimePair.Item2);
                }
            }
        }
    }

    public static void LoadSprinklerState(ReactiveState reactiveState)
    {
        ReactiveStates.Add(reactiveState);
    }
}
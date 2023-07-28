using EventLogGenerationLibrary.Models;
using EventLogGenerationLibrary.Models.Events;
using EventLogGenerationLibrary.Models.States;
using EventLogGenerationLibrary.Services;
using EventLogGenerator.Models;

namespace EventLogGenerator.Services;

internal static class FixedTimeStateService
{
    // Delegate for handling event of entering state
    internal delegate void StateEnteredHandler(object sender, StateEnteredArgs data);

    // Define event for state entering that uses the delegate above
    internal static event StateEnteredHandler StateEntered;

    // Sprinkles currently ready to be sprinkled into the process
    internal static HashSet<FixedTimeState> FixedTimeStates = new();

    private static void OnReactionAdd(FixedTimeState state, Actor actor, DateTime timeStamp)
    {
        var newEvent = new StateEnteredArgs(state, actor, timeStamp);
        StateEntered.Invoke(null, newEvent);
        // FIXME: Should the logging be done by FileManager instead?
        Console.Out.WriteLine($"[INFO] {actor.Id} Added Fixed state {state.ActivityType} - {state.Resource}");
    }

    private static void AddFixedState(FixedTimeState state, Actor actor)
    {
        var timeWithActorOffset = state.VisitTime + ActorService.GetActorActivityOffset(actor, state.ActivityType);
        OnReactionAdd(state, actor, timeWithActorOffset);
    }

    // NOTE: In ideal world, this would be probably abstracted to another state. FixedTimeState should have a single responsibility.
    internal static void RunFixedStates(ActorFrame filledActorFrame)
    {
        foreach (var state in FixedTimeStates)
        {
            if (state.MustOccur == null || filledActorFrame.VisitedMap.ContainsKey(state.MustOccur))
            {
                AddFixedState(state, filledActorFrame.Actor);
            }
        }
    }

    internal static void LoadFixedState(FixedTimeState state)
    {
        FixedTimeStates.Add(state);
    }

    internal static void ResetService()
    {
        FixedTimeStates = new();
    }
}
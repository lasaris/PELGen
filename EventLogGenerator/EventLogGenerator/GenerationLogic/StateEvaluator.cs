using EventLogGenerator.Models;
using EventLogGenerator.Models.Enums;

namespace EventLogGenerator.GenerationLogic;

/// <summary>
/// Evaluates given states and decides which states to follow next
/// </summary>
public static class StateEvaluator
{
    // Delegate for handling event of entering state
    public delegate void StateEnteredHandler(object sender, StateEnteredEvent data);

    // Define event for state entering that uses the delegate above
    public static event StateEnteredHandler StateEntered;
    
    // Current ActorFrame going through the process
    public static ActorFrame CurrentActorFrame { get; set; }
    
    // All current available states of the process
    public static HashSet<ProcessState> AllStates = new();
    
    // ProcessStates that are available for next step in the current process
    public static HashSet<ProcessState> AvailableNext = new();

    public static void AddState(ProcessState state)
    {
        if (!AllStates.Add(state))
        {
            throw new ArgumentException("State already present in the evaluator set");
        }
    }
    
    public static void RunProcess()
    {
        throw new NotImplementedException();
    }

    private static void JumpNextState(ProcessState newState, DateTime jumpDate)
    {
        // TODO: change current state and time
        
        // TODO: update AvailableNext
        
        OnStateEnter(CurrentActorFrame.Actor, newState, jumpDate);
    }

    private static void AddNextAvailable(ProcessState newState)
    {
        // Don't worry about already present states
        AvailableNext.Add(newState);
    }
    
    private static void OnStateEnter(Actor actor, ProcessState newState, DateTime enteredTime)
    {
        var eventData = new StateEnteredEvent(newState, actor, enteredTime);
        StateEntered.Invoke(null, eventData);
    }
}
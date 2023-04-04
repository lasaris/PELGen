using EventLogGenerator.Models;

namespace EventLogGenerator.GenerationLogic;

/// <summary>
/// Evaluates given states and decides which states to follow next
/// </summary>
public static class StateEvaluator
{
    // All available states of the process
    public static HashSet<ProcessState> AllStates = new();
    
    // ProcessStates that are available for next step in the process
    public static HashSet<ProcessState> AvailableNext = new();

    public static void AddState(ProcessState state)
    {
        if (!AllStates.Add(state))
        {
            throw new ArgumentException("State already present in the evaluator set");
        }
    }
    
    public static void AddNextAvailable(ProcessState newState)
    {
        if(!AvailableNext.Add(newState))
        {
            throw new ArgumentException("Cannot add new available state (already present)");
        }
    }
    
    // TODO: Running evaluator with ActorFrame
}
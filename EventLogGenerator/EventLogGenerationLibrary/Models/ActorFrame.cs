using EventLogGenerationLibrary.Models.States;

namespace EventLogGenerationLibrary.Models;

/// <summary>
/// This frame moves along states with the actor.
/// Simulates the entire process run by changing its current process states.
/// </summary>
internal class ActorFrame
{
    // Actor of the ongoing process
    internal Actor Actor;

    // Current state of the process
    internal ProcessState CurrentState;

    // Dictionary of ProcessStates that were previously visited in the process, values counts the visits
    internal Dictionary<ProcessState, int> VisitedMap = new();
    
    // Dictionary of ProcessStates that were previously visited in the process, values counts the visits
    internal Dictionary<string, int> VisitedActivitiesMap = new();
    
    // The List (stack) of all visited States and the times they were visited in
    internal List<(ProcessState, DateTime)> VisitedStack = new();

    // The time in which the frame moves. Should be updated forward after each state transition
    internal DateTime CurrentTime;

    internal ActorFrame(Actor actor, ProcessState currentState)
   {
        Actor = actor;
        CurrentState = currentState;
        CurrentTime = currentState.TimeFrame.PickTimeByDistribution();
   }
}
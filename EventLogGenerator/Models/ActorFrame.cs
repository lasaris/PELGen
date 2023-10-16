using EventLogGenerator.Models.Enums;

namespace EventLogGenerator.Models;

/// <summary>
/// This frame moves along states with the actor. Simulates the entire process
/// </summary>
public class ActorFrame
{
    // Actor of the ongoing process
    public Actor Actor;

    // Current state of the process
    public ProcessState CurrentState;

    // Dictionary of ProcessStates that were previously visited in the process, values counts the visits
    public Dictionary<ProcessState, int> VisitedMap = new();
    
    // Dictionary of ProcessStates that were previously visited in the process, values counts the visits
    public Dictionary<EActivityType, int> VisitedActivitiesMap = new();
    
    // The List (stack) of all visited States and the times they were visited in
    public List<(ProcessState, DateTime)> VisitedStack = new();

    // The time in which the frame moves. Should be updated forward after each state transition
    public DateTime CurrentTime;

    public ActorFrame(Actor actor, ProcessState currentState)
   {
        Actor = actor;
        CurrentState = currentState;
        CurrentTime = currentState.TimeFrame.PickTimeByDistribution();
   }
}
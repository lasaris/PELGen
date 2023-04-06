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

    // The previously visited state (null if currently at start)
    public ProcessState? LastVisited = null;

    // The time in which the frame moves. Should be updated forward after each state transition
    public DateTime? CurrentTime = null;

    public ActorFrame(Actor actor, ProcessState currentState)
   {
        Actor = actor;
        CurrentState = currentState;
   }
}
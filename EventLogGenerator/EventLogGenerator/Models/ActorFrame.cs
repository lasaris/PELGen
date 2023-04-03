namespace EventLogGenerator.Models;

/// <summary>
/// This frame moves along states with the actor. Simulates the entire process
/// </summary>
public class ActorFrame
{
    // Actor of the ongoing process
    public Actor Actor;

    // Current state of the process (null if process just finished)
    public ProcessState? CurrentState;

    // ProcessStates that are available for next step in the process
    public HashSet<ProcessState> AvailableNext;

    // Dictionary of ProcessStates that were previously visited in the process, values counts the visits
    public Dictionary<ProcessState, int> VisitedMap;

    // The previously visited state (null if currently at start)
    public ProcessState? LastVisited;

    // The time in which the frame moves. Should be updated forward after each state transition
    public DateTime CurrentTime;

    // TODO: Next() function to jump to next state after evaluation from StateEvaluator()
}
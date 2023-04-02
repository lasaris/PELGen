namespace EventLogGenerator.Models;

public class ProcessState
{
    // Unique ID for ProcessState (CaseID in process mining)
    public int _id;
    
    // Activity for given state
    public Activity Activity;

    // Rules which apply for given state and which lead to the next
    public StateRules Rules;

    // Chances properties for the upcoming move decisions
    public StateChances Chances;

    // Start and end time in between which the state can be visited
    public TimeFrame TimeFrame;
    
    // Indicates if this is the starting state of process (should be only one)
    public bool IsStarting;

    // Indicates if process is finished with this state (can be multiple states)
    public bool IsFinishing;
}
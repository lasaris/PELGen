namespace EventLogGenerator.Models;

public class ProcessCase
{
    // Unique ID for ProcessState
    public int _id;
    
    // Activity for given state
    public Activity Activity;

    // Rules which apply for given state and which lead to the next
    public StateRules Rules;
    
    // Chances 

    // Indicates if this is the starting state of process (should be only one)
    public bool IsStarting;

    // Indicates if process is finished with this state (can be multiple states)
    public bool IsFinishing;
}
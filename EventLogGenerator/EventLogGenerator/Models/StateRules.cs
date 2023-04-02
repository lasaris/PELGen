namespace EventLogGenerator.Models;

public class StateRules
{
    // Defines if Actor must perform this activity in the process
    public bool IsCompulsory;

    // Defines if Actor can return to this same Activity (create loop over it)
    public bool CanRepeat;

    // If actor can repeat the same activity, how many times can it loop
    public int MaxRepetitions;
    
    // How many times can this activity be performed in the process (-1 = unlimited times)
    public int MaxPasses;

    // ProcessStates that must be visited by Actor before the current one (can be indirect)
    public List<ProcessCase> MustPreceed;
    
    // ProcessState that must be a direct parent of current one
    public ProcessCase DirectParent;
}
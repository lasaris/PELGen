using EventLogGenerator.Models.Enums;

namespace EventLogGenerator.Models;

public class StateRules
{
    // Defines if Actor must perform this activity in the process
    public bool IsCompulsory;

    // How many times can this activity be performed in the process (-1 = unlimited times)
    public int MaxPasses;

    // If actor can repeat the same activity, how many times can it loop (-1 = unlimited times)
    public int MaxLoops;

    // ProcessStates that must be visited by Actor before the current one (can be indirect, null if no limitations)
    public HashSet<ProcessState>? MustPrecedeStates;

    // Activities that must be visited by Actor before the current one (can be indirect, null if no limitations)
    public HashSet<EActivityType>? MustPrecedeActivities;

    public StateRules(bool isCompulsory = false, int maxPasses = 1, int maxLoops = 0, HashSet<ProcessState>? mustPrecedeStates = null,
        HashSet<EActivityType>? mustPrecedeActivities = null)
    {
        IsCompulsory = isCompulsory;
        MaxPasses = maxPasses;
        MaxLoops = maxLoops;
        MustPrecedeStates = mustPrecedeStates;
        MustPrecedeActivities = mustPrecedeActivities;
    }
}
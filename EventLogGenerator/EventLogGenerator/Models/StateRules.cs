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

    // Following activity type and its chance to follow as next one 
    public (EActivityType, float)[]? FollowingActivitiesMap;

    // ProcessStates that must be visited by Actor before the current one (can be indirect, null if no limitations)
    public HashSet<ProcessState>? MustPreceedStates;

    // Activities that must be visited by Actor before the current one (can be indirect, null if no limitations)
    public HashSet<EActivityType>? MustPreceedActivities;

    // ProcessState that must be a direct parent of the current one (null if not required)
    public ProcessState? DirectParent;

    public StateRules(bool isCompulsory = false, int maxPasses = 1, int maxLoops = 0,
        (EActivityType, float)[]? followingActivitiesMap = null, HashSet<ProcessState>? mustPreceedStates = null,
        HashSet<EActivityType>? mustPreceedActivities = null, ProcessState? directParent = null)
    {
        IsCompulsory = isCompulsory;
        MaxPasses = maxPasses;
        MaxLoops = maxLoops;
        FollowingActivitiesMap = followingActivitiesMap;
        MustPreceedStates = mustPreceedStates;
        MustPreceedActivities = mustPreceedActivities;
        DirectParent = directParent;
    }
}
using EventLogGenerator.Models.Enums;
using EventLogGenerator.Models.States;

namespace EventLogGenerator.Models;

public delegate void AdditionalActionFunc(Actor currentActor);

public class ProcessState : ABaseState
{
    // Rules which apply for given state and which lead to the next
    public StateRules Rules;

    // Start and end time in between which the state can be visited
    public TimeFrame TimeFrame;

    // Following states and their changes
    public Dictionary<ProcessState, float> FollowingMap;

    // Indicates if process is finished with this state (can be multiple states)
    public bool IsFinishing;

    public AdditionalActionFunc? Callback;

    public ProcessState(EActivityType activity, Resource resource, StateRules rules, TimeFrame timeFrame,
        bool isFinishing = false, AdditionalActionFunc? callback = null) : base(activity, resource)
    {
        Rules = rules;
        TimeFrame = timeFrame;
        IsFinishing = isFinishing;
        FollowingMap = new();
        Callback = callback;
    }

    public void AddFollowingState(ProcessState state, float chance)
    {
        if (chance < 0 || chance > 1)
        {
            throw new ArgumentException("Chance must be between 0 and 1 values");
        }

        if (!FollowingMap.TryAdd(state, chance))
        {
            throw new ArgumentException("Keypair could not be added. Perhaps the key already exists?");
        }
    }
    
    public void AddFollowingStates(params (ProcessState, float)[] tuples)
    {
        foreach (var pair in tuples)
        {
            AddFollowingState(pair.Item1, pair.Item2);
        }
    }
}
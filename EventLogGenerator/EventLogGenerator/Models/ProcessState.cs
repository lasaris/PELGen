using EventLogGenerator.GenerationLogic;
using EventLogGenerator.Models.Enums;

namespace EventLogGenerator.Models;

public class ProcessState
{
    // Activity for given state
    public EActivityType ActivityType;

    // Resource used to accomplish activity of the state
    public Resource Resource;

    // Rules which apply for given state and which lead to the next
    public StateRules Rules;

    // Start and end time in between which the state can be visited
    public TimeFrame TimeFrame;

    // Following states and their changes
    public Dictionary<ProcessState, float> FollowingMap;

    // Indicates if process is finished with this state (can be multiple states)
    public bool IsFinishing;

    public ProcessState(EActivityType activity, Resource resource, StateRules rules, TimeFrame timeFrame,
        bool isFinishing = false)
    {
        ActivityType = activity;
        Resource = resource;
        Rules = rules;
        TimeFrame = timeFrame;
        IsFinishing = isFinishing;
        FollowingMap = new();
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
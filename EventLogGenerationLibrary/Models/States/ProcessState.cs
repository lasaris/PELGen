using EventLogGenerator.Models;

namespace EventLogGenerationLibrary.Models.States;

public delegate void AdditionalActionFunc(Actor currentActor);

/// <summary>
/// The main building block for the process.
/// It has a defined TimeFrame in which it has to occur. The FollowingMap specifies which states can be visited
/// after this one with float value as the chance to visit that specific state (which should be between 0 and 1).
/// IsFinishing indicates if the whole process should finish with this state.
/// MaxPasses defines the maximum amount of passes through current state, which can also loop on itself.
/// </summary>
public class ProcessState : ABaseState
{
    // Start and end time in between which the state can be visited
    public TimeFrame TimeFrame;

    // Following states and their changes
    public Dictionary<ProcessState, float> FollowingMap;

    // Indicates if process is finished with this state (can be multiple states)
    public bool IsFinishing;
    
    // Rules which apply for given state and which lead to the next
    public int MaxPasses;

    // NOTE: this could be designed more general with better abstraction.
    public AdditionalActionFunc? Callback;

    public ProcessState(string activity, string resource, int maxPasses, TimeFrame timeFrame,
        bool isFinishing = false, AdditionalActionFunc? callback = null) : base(activity, resource)
    {
        TimeFrame = timeFrame;
        IsFinishing = isFinishing;
        FollowingMap = new();
        MaxPasses = maxPasses;
        Callback = callback;
    }
    
    public ProcessState(string activity, string resource, int maxPasses, DateTime start,
        bool isFinishing = false, AdditionalActionFunc? callback = null) : base(activity, resource)
    {
        TimeFrame = new TimeFrame(start, start + TimeSpan.FromSeconds(1));
        IsFinishing = isFinishing;
        FollowingMap = new();
        MaxPasses = maxPasses;
        Callback = callback;
    }

    public void AddFollowingState(ProcessState state, float chance = 1)
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
﻿using EventLogGenerator.Models.Enums;
using EventLogGenerator.Models.States;

namespace EventLogGenerator.Models;

public delegate void AdditionalActionFunc(Actor currentActor);

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

    // FIXME: Design this better and more generally!
    public AdditionalActionFunc? Callback;

    public ProcessState(EActivityType activity, Resource resource, int maxPasses, TimeFrame timeFrame,
        bool isFinishing = false, AdditionalActionFunc? callback = null) : base(activity, resource)
    {
        TimeFrame = timeFrame;
        IsFinishing = isFinishing;
        FollowingMap = new();
        MaxPasses = maxPasses;
        Callback = callback;
    }
    
    public ProcessState(EActivityType activity, Resource resource, int maxPasses, DateTime start,
        bool isFinishing = false, AdditionalActionFunc? callback = null) : base(activity, resource)
    {
        TimeFrame = new TimeFrame(start, start + TimeSpan.FromSeconds(1));
        IsFinishing = isFinishing;
        FollowingMap = new();
        MaxPasses = maxPasses;
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
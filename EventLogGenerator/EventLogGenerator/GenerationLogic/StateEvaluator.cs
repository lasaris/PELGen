using EventLogGenerator.Exceptions;
using EventLogGenerator.Models;
using EventLogGenerator.Services;

namespace EventLogGenerator.GenerationLogic;

/// <summary>
/// Evaluates given states and decides which states to follow next
/// </summary>
public static class StateEvaluator
{
    // Delegate for handling event of entering state
    public delegate void StateEnteredHandler(object sender, StateEnteredEvent data);

    // Define event for state entering that uses the delegate above
    public static event StateEnteredHandler StateEntered;

    // Current ActorFrame going through the process
    public static ActorFrame? CurrentActorFrame { get; set; }

    // The time when process has to be finished. Can be used as emergency limit
    public static DateTime? ProcessEnd { get; set; }

    private static readonly Random Random = new();

    public static void RunProcess()
    {
        // Check for desired state of evaluator
        if (CurrentActorFrame == null)
        {
            throw new Exception("ActorFrame must be set before running the evaluator");
        }

        if (ProcessEnd == null)
        {
            throw new Exception("ProcessEnd must be set before running the evaluator");
        }
        
        Console.WriteLine("[INFO] --- PROCESS RUN STARTED---");
        // Running loop
        while (!CurrentActorFrame.CurrentState.IsFinishing)
        {
            // Get available following states of current states
            var currentState = CurrentActorFrame.CurrentState;
            var availableStates = currentState.FollowingMap.Keys.ToList();

            if (!availableStates.Any())
            {
                throw new InvalidProcessStateException("Following map must contain following states");
            }

            // Evaluate weight of the states
            Dictionary<ProcessState, float> weightedStates = new();
            foreach (var state in availableStates)
            {
                // Skip states that end before our current time
                if (CurrentActorFrame.CurrentTime > state.TimeFrame.End)
                {
                    continue;
                }

                // Skip states that are at maximum amount of passes
                if (CurrentActorFrame.VisitedMap.ContainsKey(state)
                    && CurrentActorFrame.VisitedMap[state] == state.Rules.MaxPasses)
                {
                    continue;
                }

                float rating = 0;

                // Rank following chance
                rating += currentState.FollowingMap[state] * Constants.ChanceToFollowWeight;
                
                // FIXME: Remove this old ranking by state parameters
                // // Rank compulsory
                //
                // if (state.Rules.IsCompulsory)
                // {
                //     rating += Constants.CompulsoryWeight;
                // }
                //
                // // Rank activity chance
                // if (currentState.ActivityType != state.ActivityType)
                // {
                //     rating += Constants.DifferentActivityWeight;
                // }
                //
                // // Rank same resource
                // if (currentState.Resource == state.Resource)
                // {
                //     rating += Constants.SameResourceWeight;
                // }
                //
                // // Rank finishing state
                // if (state.IsFinishing)
                // {
                //     rating += Constants.ToFinishingWeight;
                // }
                //
                // // Penalize previous visit
                // if (CurrentActorFrame.VisitedMap.ContainsKey(state))
                // {
                //     rating = Math.Max(0, CurrentActorFrame.VisitedMap[state] * Constants.EachPreviousVisitWeight + rating);
                // }
                //
                // // Penalize last visited
                // if (CurrentActorFrame.LastVisited != null && CurrentActorFrame.LastVisited.Equals(state))
                // {
                //     rating = Math.Max(0, rating + Constants.LastVisitWeight);
                // }

                weightedStates.Add(state, rating);
            }
            
            if (!weightedStates.Any())
            {
                throw new InvalidProcessStateException(
                    "Weighted states are empty, this can happen because all following states end sooner than our current state");
            }
            
            var nextState = SelectWeightedState(weightedStates);
            var jumpTime = nextState.TimeFrame.PickTimeByDistribution(CurrentActorFrame.CurrentTime);
            var actorOffset = ActorService.GetActorActivityOffset(CurrentActorFrame.Actor, currentState.ActivityType);;
            JumpNextState(nextState, jumpTime, actorOffset);
        }

        Console.Out.WriteLine("[INFO] --- ENDING PROCESS RUN ---");
    }

    private static void JumpNextState(ProcessState newState, DateTime jumpDate, TimeSpan actorOffset)
    {
        // Update VisitedMap
        if (CurrentActorFrame.VisitedMap.ContainsKey(CurrentActorFrame.CurrentState))
        {
            CurrentActorFrame.VisitedMap[CurrentActorFrame.CurrentState] += 1;
        }
        else
        {
            CurrentActorFrame.VisitedMap.Add(CurrentActorFrame.CurrentState, 1);
        }

        // Update loop count or current state to the new one
        if (CurrentActorFrame.CurrentState.Equals(newState))
        {
            CurrentActorFrame.CurrentLoopCount += 1;
        }
        else
        {
            CurrentActorFrame.LastVisited = CurrentActorFrame.CurrentState;
            CurrentActorFrame.CurrentState = newState;
        }
        
        CurrentActorFrame.CurrentTime = jumpDate;
        CurrentActorFrame.VisitedStack.Add((newState, jumpDate));
        OnStateEnter(CurrentActorFrame.Actor, newState, jumpDate + actorOffset);
    }

    public static ProcessState SelectWeightedState(Dictionary<ProcessState, float> stateWeights)
    {
        float totalWeight = stateWeights.Values.Sum();
        float randomWeight = (float)Random.NextDouble() * totalWeight;
        float cumulativeWeight = 0f;

        foreach (var kvp in stateWeights)
        {
            cumulativeWeight += kvp.Value;
            if (randomWeight < cumulativeWeight)
            {
                return kvp.Key;
            }
        }
        
        throw new InvalidOperationException("Cannot pick from empty state-weight pairs");
    }

    public static void OnStateEnter(Actor actor, ProcessState newState, DateTime enteredTime)
    {
        Console.Out.WriteLine($"[INFO] {actor.Id} Entering state: {newState.ActivityType} {newState.Resource.Name}");
        var eventData = new StateEnteredEvent(newState, actor, enteredTime);
        StateEntered.Invoke(null, eventData);
    }

    public static void InitializeEvaluator(ActorFrame actorFrame, DateTime processEnd)
    {
        CurrentActorFrame = actorFrame;
        ProcessEnd = processEnd;
    }
}
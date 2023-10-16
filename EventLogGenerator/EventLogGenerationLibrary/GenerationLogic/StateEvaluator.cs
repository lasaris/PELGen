using EventLogGenerationLibrary.Models;
using EventLogGenerationLibrary.Models.Events;
using EventLogGenerationLibrary.Models.States;
using EventLogGenerationLibrary.Services;
using EventLogGenerator.Exceptions;
using EventLogGenerator.Services;

namespace EventLogGenerationLibrary.GenerationLogic;

/// <summary>
/// Evaluates given states and decides which states to follow next
/// </summary>
internal static class StateEvaluator
{
    // Delegate for handling event of entering state
    internal delegate void StateEnteredHandler(object sender, StateEnteredArgs data);

    // Define event for state entering that uses the delegate above
    internal static event StateEnteredHandler StateEntered;

    // Current ActorFrame going through the process
    internal static ActorFrame? CurrentActorFrame { get; set; }

    // The constraints to different activities (maximum occurences in a single process)
    internal static Dictionary<string, int> ActivitiesLimits = new();

    internal static ActorFrame RunProcess(ActorFrame actorFrame)
    {
        CurrentActorFrame = actorFrame;

        // Check for desired state of evaluator
        if (CurrentActorFrame == null)
        {
            throw new Exception("ActorFrame must be set before running the evaluator");
        }

        Console.WriteLine("[INFO] --- PROCESS RUN STARTED---");
        JumpNextState(CurrentActorFrame.CurrentState, CurrentActorFrame.CurrentTime, TimeSpan.Zero);

        // Running loop
        while (!CurrentActorFrame.CurrentState.IsFinishing)
        {
            // Get available following states of current states
            var currentState = CurrentActorFrame.CurrentState;
            var availableStates = currentState.FollowingMap.Keys.ToList();

            if (!availableStates.Any())
            {
                throw new InvalidProcessStateException(
                    "Following map must contain following states. Perhaps some state is missing following states or no finishing state is defined?");
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
                    && CurrentActorFrame.VisitedMap[state] == state.MaxPasses)
                {
                    continue;
                }

                // Skip states with activities that reached its limit
                var newActivity = state.ActivityType;
                if (CurrentActorFrame.VisitedActivitiesMap.ContainsKey(newActivity)
                    && ActivitiesLimits.ContainsKey(newActivity)
                    && CurrentActorFrame.VisitedActivitiesMap[newActivity] == ActivitiesLimits[newActivity])
                {
                    continue;
                }

                float rating = 0;

                // Rank following chance
                rating += currentState.FollowingMap[state];
                weightedStates.Add(state, rating);
            }

            if (!weightedStates.Any())
            {
                throw new InvalidProcessStateException(
                    "Weighted states are empty, this can happen because all following states end sooner than our current state");
            }

            var nextState = SelectWeightedState(weightedStates);
            var jumpTime = nextState.TimeFrame.PickTimeByDistribution(CurrentActorFrame.CurrentTime);
            var actorOffset = ActorService.GetActorActivityOffset(CurrentActorFrame.Actor, nextState.ActivityType);
            JumpNextState(nextState, jumpTime, actorOffset);
        }

        Console.Out.WriteLine("[INFO] --- ENDING PROCESS RUN ---");
        return CurrentActorFrame;
    }

    private static void JumpNextState(ProcessState state, DateTime jumpDate, TimeSpan offset)
    {
        // Perform callback function if possible
        if (state.Callback != null)
        {
            state.Callback(CurrentActorFrame.Actor);
        }

        // Update VisitedMap
        if (CurrentActorFrame.VisitedMap.ContainsKey(CurrentActorFrame.CurrentState))
        {
            CurrentActorFrame.VisitedMap[CurrentActorFrame.CurrentState] += 1;
        }
        else
        {
            CurrentActorFrame.VisitedMap.Add(CurrentActorFrame.CurrentState, 1);
        }

        // Update VisitedActivities
        if (CurrentActorFrame.VisitedActivitiesMap.ContainsKey(CurrentActorFrame.CurrentState.ActivityType))
        {
            CurrentActorFrame.VisitedActivitiesMap[CurrentActorFrame.CurrentState.ActivityType] += 1;
        }
        else
        {
            CurrentActorFrame.VisitedActivitiesMap.Add(CurrentActorFrame.CurrentState.ActivityType, 1);
        }

        // Update CurrentState
        CurrentActorFrame.CurrentState = state;
        CurrentActorFrame.CurrentTime = jumpDate;
        CurrentActorFrame.VisitedStack.Add((state, jumpDate + offset));
        OnStateEnter(CurrentActorFrame.Actor, state, jumpDate + offset);
    }

    internal static ProcessState SelectWeightedState(Dictionary<ProcessState, float> stateWeights)
    {
        float totalWeight = stateWeights.Values.Sum();
        float randomWeight = (float)RandomService.GetNextDouble() * totalWeight;
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

    internal static void OnStateEnter(Actor actor, ProcessState newState, DateTime enteredTime)
    {
        Console.Out.WriteLine($"[INFO] {actor.Id} Entering state: {newState.ActivityType} {newState.Resource}");
        var eventData = new StateEnteredArgs(newState, actor, enteredTime);
        StateEntered.Invoke(null, eventData);
    }

    internal static void SetLimits(Dictionary<string, int> activitiesLimits)
    {
        ActivitiesLimits = activitiesLimits;
    }
}
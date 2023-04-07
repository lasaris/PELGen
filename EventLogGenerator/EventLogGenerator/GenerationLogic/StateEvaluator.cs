﻿using EventLogGenerator.Models;
using EventLogGenerator.Models.Enums;

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

    // All current available states of the process
    public static HashSet<ProcessState> AllStates = new();

    // The time when process has to be finished. Can be used as emergency limit
    public static DateTime? ProcessEnd { get; set; }

    private static readonly Random random = new();

    public static void AddState(ProcessState state)
    {
        if (!AllStates.Add(state))
        {
            throw new ArgumentException("State already present in the evaluator set");
        }
    }

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
            Console.WriteLine(
                $"[INFO] Current state {CurrentActorFrame.CurrentState.ActivityType} - {CurrentActorFrame.CurrentState.Resource.Name}");
            // Create copy of all states
            HashSet<ProcessState> allStatesCopy = new HashSet<ProcessState>(AllStates);

            // Alias for shorter code
            var currentState = CurrentActorFrame.CurrentState;

            // From the copy, remove states that cannot be visited
            foreach (var state in AllStates)
            {
                // Remove states with invalid time
                if (CurrentActorFrame.CurrentTime < state.TimeFrame.Start
                    || CurrentActorFrame.CurrentTime >= state.TimeFrame.End)
                {
                    allStatesCopy.Remove(state);
                    continue;
                }

                // Remove states where must precede states were not yet visited
                if (state.Rules.MustPrecedeStates != null)
                {
                    foreach (var mustVisit in state.Rules.MustPrecedeStates)
                    {
                        if (!CurrentActorFrame.VisitedMap.Keys.Contains(mustVisit)
                            && !CurrentActorFrame.CurrentState.Equals(mustVisit))
                        {
                            allStatesCopy.Remove(state);
                        }
                    }
                }

                // Remove states where must precede activities were not yet visited
                if (state.Rules.MustPrecedeActivities != null)
                {
                    foreach (var mustPerform in state.Rules.MustPrecedeActivities)
                    {
                        if (CurrentActorFrame.VisitedMap.Keys.All(s => s.ActivityType != mustPerform)
                            && CurrentActorFrame.CurrentState.ActivityType != mustPerform)
                        {
                            allStatesCopy.Remove(state);
                        }
                    }
                }

                // Remove states which have direct parent defined and it is not our current state
                if (state.Rules.DirectParent != null && state.Rules.DirectParent != currentState)
                {
                    allStatesCopy.Remove(state);
                    continue;
                }

                // Remove current state, if MaxLoops == 0 or current loop count is at maximum
                if (currentState.Equals(state)
                    && (state.Rules.MaxLoops == 0 || CurrentActorFrame.CurrentLoopCount == currentState.Rules.MaxLoops))
                {
                    allStatesCopy.Remove(state);
                }
            }

            // Handle empty states
            if (!allStatesCopy.Any())
            {
                CurrentActorFrame.CurrentTime = CurrentActorFrame.CurrentTime.AddMinutes(10);
                if (CurrentActorFrame.CurrentTime >= ProcessEnd)
                {
                    Console.WriteLine($"[INFO] Current time reached the end time process end {currentState.ActivityType} {currentState.Resource}");
                     break;
                }

                continue;
            }

            Dictionary<ProcessState, float> weightedStates = new();

            // Evaluate remaining states
            foreach (var state in allStatesCopy)
            {
                float rating = 0;

                // Rank compulsory
                if (state.Rules.IsCompulsory)
                {
                    rating += Constants.CompulsoryWeight;
                }

                // Rank following activities
                if (currentState.Rules.FollowingActivitiesMap != null
                    && currentState.Rules.FollowingActivitiesMap.Keys.Contains(state.ActivityType))
                {
                    rating += currentState.Rules.FollowingActivitiesMap[state.ActivityType] *
                              Constants.ChanceToFollowWeight;
                }

                // Rank chance to visit
                rating += state.Chances.ChanceToVisit * Constants.ChanceToVisitWeight;

                // Rank activity chance
                if (currentState.ActivityType != state.ActivityType)
                {
                    rating += Constants.DifferentActivityWeight;
                }

                // Penalize previous visit
                if (CurrentActorFrame.VisitedMap.Keys.Contains(state))
                {
                    rating += CurrentActorFrame.VisitedMap[state] * Constants.EachPreviousVisitWeight;
                }

                // Penalize last visited
                if (CurrentActorFrame.LastVisited != null && CurrentActorFrame.LastVisited.Equals(state))
                {
                    rating += Constants.LastVisitWeight;
                }

                // Rank same resource
                if (currentState.Resource == state.Resource)
                {
                    rating += Constants.SameResourceWeight;
                }

                // Rank finishing state
                if (state.IsFinishing)
                {
                    rating += Constants.ToFinishingWeight;
                }

                weightedStates.Add(state, rating);
            }

            var nextState = SelectWeightedState(weightedStates);
            JumpNextState(nextState, nextState.TimeFrame.PickTimeByDistribution(CurrentActorFrame.CurrentTime));
        }
        Console.Out.WriteLine("[INFO] --- ENDING PROCESS RUN ---");
    }

    private static void JumpNextState(ProcessState newState, DateTime jumpDate)
    {
        // Update VisitedMap
        if (CurrentActorFrame.VisitedMap.Keys.Contains(CurrentActorFrame.CurrentState))
        {
            CurrentActorFrame.VisitedMap[CurrentActorFrame.CurrentState] += 1;
        }
        else
        {
            CurrentActorFrame.VisitedMap.Add(newState, 1);
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
        OnStateEnter(CurrentActorFrame.Actor, newState, jumpDate);
    }

    public static ProcessState SelectWeightedState(Dictionary<ProcessState, float> stateWeights)
    {
        float totalWeight = stateWeights.Values.Sum();
        float randomWeight = (float)random.NextDouble() * totalWeight;
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

    private static void OnStateEnter(Actor actor, ProcessState newState, DateTime enteredTime)
    {
        var eventData = new StateEnteredEvent(newState, actor, enteredTime);
        StateEntered.Invoke(null, eventData);
    }

    public static void InitializeEvaluator(ActorFrame actorFrame, DateTime processEnd)
    {
        CurrentActorFrame = actorFrame;
        ProcessEnd = processEnd;
    }
}
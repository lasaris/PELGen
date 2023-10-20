using EventLogGenerationLibrary.Models;
using EventLogGenerationLibrary.Models.Events;
using EventLogGenerationLibrary.Models.States;
using EventLogGenerator.Models;
using EventLogGenerator.Services;

namespace EventLogGenerationLibrary.Services;

/// <summary>
/// Handles reactive states and scenarios operations.
/// </summary>
internal static class ReactiveStateService
{
    // Delegate for handling event of entering state
    internal delegate void StateEnteredHandler(object sender, StateEnteredArgs data);

    // Define event for state entering that uses the delegate above
    internal static event StateEnteredHandler StateEntered;

    // Sprinkles currently ready to be sprinkled into the process
    internal static HashSet<ReactiveState> ReactiveStates = new();

    // Maps actors that are reacted to actors that are reacting to them
    internal static Dictionary<Actor, Actor>? ReactingActorsMap = null;

    internal static Actor? SingleReactingActor = null; 
    
    // Stores reactive scnearios
    internal static HashSet<PatternReaction> PatternReactions = new();

    private static void OnStateEnter(ABaseState state, Actor actor, DateTime timeStamp, string? additional = null)
    {
        var newEvent = new StateEnteredArgs(state, actor, timeStamp, additional);
        StateEntered.Invoke(null, newEvent);
        Console.Out.WriteLine($"[INFO] {actor.Id} Added Reactive state {state.ActivityType} - {state.Resource}");
    }

    private static void AddReactiveState(ABaseState state, DateTime reactionTime, Actor actor)
    {
        if (SingleReactingActor != null)
        {
            OnStateEnter(state, SingleReactingActor, reactionTime, actor.Id.ToString());
        }
        else if (ReactingActorsMap != null)
        {
            OnStateEnter(state, ReactingActorsMap[actor], reactionTime, actor.Id.ToString());
        }
        else
        {
            throw new Exception("Invalid state of Reactive state service.");
        }
    }

    internal static void RunReactiveStates(Process? previousProcess, ReactingActorStrategy reactingStrategy)
    {
        // If no previous process provided or no reactive states available, just quit
        if (previousProcess == null || !ReactiveStates.Any())
        {
            return;
        }

        if (reactingStrategy.SingleReactingActor != null)
        {
            SingleReactingActor = reactingStrategy.SingleReactingActor;
        }
        else if (reactingStrategy.AssignActorsFunction != null)
        {
            ReactingActorsMap = reactingStrategy.AssignActorsFunction(previousProcess);
        }
        else
        {
            throw new ArgumentException("Invalid Reacting actor strategy provided into Reactive state service.");
        }
        
        // Adding the reactive states
        foreach (var actorStatesPair in previousProcess.Log)
        {
            foreach (var record in actorStatesPair.Value.Trace)
            {
                foreach (var state in ReactiveStates)
                {
                    if (state.ReactToActivity == record.State.ActivityType)
                    {
                        if (state.ReactToResourceName != null &&
                            record.State.Resource != state.ReactToResourceName)
                        {
                            continue;
                        }

                        if (state.OwnResource != null)
                        {
                            state.Resource = state.OwnResource;
                        }
                        else
                        {
                            state.Resource = record.State.Resource;
                        }

                        var variableDirection = RandomService.GetNext(2) == 0 ? 1 : -1;
                        var variableTime = TimeSpan.FromTicks((long)(RandomService.GetNextDouble() *
                                                                     (state.TimeVariable.Ticks) * variableDirection));
                        var reactionTime = record.Time + state.Offset + variableTime;

                        var newState = new DummyState(state.ActivityType, state.Resource);
                        AddReactiveState(newState, reactionTime, actorStatesPair.Key);
                    }
                }
            }
        }
        
        // Execute reactive scenario
        foreach (var actorStatesPair in previousProcess .Log)
        {
            foreach (var patternReaction in PatternReactions)
            {
                var stateTimePairs = actorStatesPair.Value;

                int scenarioIndex = 0;
                for (int i = 0; i < stateTimePairs.Trace.Count; i++)
                {
                    var stateTimePair = stateTimePairs[i];

                    if (stateTimePair.State.ActivityType == patternReaction.ActivitiesPattern[scenarioIndex])
                    {
                        ++scenarioIndex;
                    }
                    else
                    {
                        scenarioIndex = 0;
                    }

                    // found the scenario to be matching
                    if (scenarioIndex == patternReaction.ActivitiesPattern.Count)
                    {
                        int firstMatchedIndex = i - (scenarioIndex - 1);

                        for (int j = firstMatchedIndex; j < firstMatchedIndex + patternReaction.ActivitiesPattern.Count; j++)
                        {
                            if (stateTimePairs[j].State.ActivityType == patternReaction.MatchTimeWith)
                            {
                                var newState = new DummyState(patternReaction.Reaction, stateTimePairs[j].State.Resource);
                                
                                AddReactiveState(newState, stateTimePair.Time + patternReaction.Offset, actorStatesPair.Key);
                                break;
                            }
                        }
                        
                        scenarioIndex = 0;
                    }
                }
            }
        }
    }

    internal static void LoadReactiveState(ReactiveState state)
    {
        ReactiveStates.Add(state);
    }

    internal static void LoadReactiveScenario(PatternReaction scenario)
    {
        PatternReactions.Add(scenario);
    }

    internal static void ResetService()
    {
        ReactiveStates = new();
        ReactingActorsMap = null;
        SingleReactingActor = null;
    }
}
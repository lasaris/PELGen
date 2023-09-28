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
    internal static Dictionary<Actor, Actor> ReactingActorsMap = new();
    
    // Stores reactive scnearios
    internal static HashSet<PatternReaction> ReactiveScenarios = new();

    private static void OnStateEnter(ABaseState state, Actor actor, DateTime timeStamp, string? additional = null)
    {
        var newEvent = new StateEnteredArgs(state, actor, timeStamp, additional);
        StateEntered.Invoke(null, newEvent);
        // FIXME: Should the logging be done by FileManager instead?
        Console.Out.WriteLine($"[INFO] {actor.Id} Added Reactive state {state.ActivityType} - {state.Resource}");
    }

    private static void AddReactiveState(ABaseState state, DateTime reactionTime, Actor actor)
    {
        // FIXME: The adding of additional column should be generalized. Perhaps not always you want to add additional ID as StudentId column data?
        OnStateEnter(state, ReactingActorsMap[actor], reactionTime, actor.Id.ToString());
    }

    internal static void RunReactiveStates(Process idToStatesMap,
        List<Actor> actors)
    {
        // FIXME: Generalize, perhaps by adding extra parameter and intiializing ReactingActorsMap?
        foreach (var actorStatePair in idToStatesMap.Log)
        {
            var seminarGroupId = -1;

            foreach (var stateTimePair in actorStatePair.Value.Trace)
            {
                switch (stateTimePair.Item1.Resource)
                {
                    case "Seminar group 1":
                        seminarGroupId = 1;
                        break;
                    case "Seminar group 2":
                        seminarGroupId = 2;
                        break;
                    case "Seminar group 3":
                        seminarGroupId = 3;
                        break;
                }
            }

            if (seminarGroupId == -1)
            {
                throw new ArgumentException("Every student must be signed to seminar group");
            }

            switch (seminarGroupId)
            {
                case 1:
                    ReactingActorsMap[actorStatePair.Key] = actors[0];
                    break;
                case 2:
                    ReactingActorsMap[actorStatePair.Key] = actors[1];
                    break;
                case 3:
                    ReactingActorsMap[actorStatePair.Key] = actors[2];
                    break;
            }
        }
        
        // Adding the reactive states
        foreach (var actorStatesPair in idToStatesMap.Log)
        {
            foreach (var stateTimePair in actorStatesPair.Value.Trace)
            {
                foreach (var state in ReactiveStates)
                {
                    if (state.ReactToActivity == stateTimePair.Item1.ActivityType)
                    {
                        if (state.ReactToResourceName != null && stateTimePair.Item1.Resource != state.ReactToResourceName)
                        {
                            continue;
                        }
                        
                        if (state.OwnResource != null)
                        {
                            state.Resource = state.OwnResource;
                        }
                        else
                        {
                            state.Resource = stateTimePair.Item1.Resource;
                        }
                        var variableDirection = RandomService.GetNext(2) == 0 ? 1 : -1;
                        var variableTime = TimeSpan.FromTicks((long)(RandomService.GetNextDouble() * (state.TimeVariable.Ticks) * variableDirection));
                        var reactionTime = stateTimePair.Item2 + state.Offset + variableTime;

                        // FIXME: Without this new dummy state, the whole thing gets fucked up and everything is just Resource == Seminar week 6 and IDK WHY
                        var newState = new DummyState(state.ActivityType, state.Resource);
                        AddReactiveState(newState, reactionTime, actorStatesPair.Key);
                    }
                }
            }
        }
        
        // Execute reactive scenario
        foreach (var actorStatesPair in idToStatesMap.Log)
        {
            foreach (var scenario in ReactiveScenarios)
            {
                var stateTimePairs = actorStatesPair.Value;

                int scenarioIndex = 0;
                for (int i = 0; i < stateTimePairs.Trace.Count; i++)
                {
                    var stateTimePair = stateTimePairs[i];

                    if (stateTimePair.Item1.ActivityType == scenario.ActivitiesPattern[scenarioIndex])
                    {
                        ++scenarioIndex;
                    }
                    else
                    {
                        scenarioIndex = 0;
                    }

                    // found the scenario to be matching
                    if (scenarioIndex == scenario.ActivitiesPattern.Count)
                    {
                        int firstMatchedIndex = i - (scenarioIndex - 1);

                        for (int j = firstMatchedIndex; j < firstMatchedIndex + scenario.ActivitiesPattern.Count; j++)
                        {
                            if (stateTimePairs[j].Item1.ActivityType == scenario.MatchTimeWith)
                            {
                                var newState = new DummyState(scenario.Reaction, stateTimePairs[j].Item1.Resource);
                                
                                // FIXME: This timeSpan offset for ropot session deletion is artificial and should be stored within scenario
                                AddReactiveState(newState, stateTimePair.Item2 + TimeSpan.FromSeconds(20), actorStatesPair.Key);
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
        ReactiveScenarios.Add(scenario);
    }

    internal static void ResetService()
    {
        ReactiveStates = new();
        ReactingActorsMap = new();
    }
}
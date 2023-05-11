using EventLogGenerator.Models;
using EventLogGenerator.Models.Enums;
using EventLogGenerator.Models.Events;
using EventLogGenerator.Models.States;

namespace EventLogGenerator.Services;

public static class ReactiveStateService
{
    // Delegate for handling event of entering state
    public delegate void StateEnteredHandler(object sender, StateEnteredEvent data);

    // Define event for state entering that uses the delegate above
    public static event StateEnteredHandler StateEntered;

    // Sprinkles currently ready to be sprinkled into the process
    public static HashSet<ReactiveState> ReactiveStates = new();

    // Maps IDs of actors that are reacted to actors that are reacting to them
    public static Dictionary<uint, Actor> ReactingActorsMap = new();
    
    // Stores reactive scnearios
    public static HashSet<ReactiveScenario> ReactiveScenarios = new();

    private static void OnStateEnter(ABaseState state, Actor actor, DateTime timeStamp, string? additional = null)
    {
        var newEvent = new StateEnteredEvent(state, actor, timeStamp, additional);
        StateEntered.Invoke(null, newEvent);
        // FIXME: Should the logging be done by EventLogger instead?
        Console.Out.WriteLine($"[INFO] {actor.Id} Added Reactive state {state.ActivityType} - {state.Resource.Name}");
    }

    private static void AddReactiveState(ABaseState reactiveState, DateTime reactionTime, uint actorId)
    {
        // FIXME: The adding of additional column should be generalized. Perhaps not always you want to add additional ID as StudentId column data?
        OnStateEnter(reactiveState, ReactingActorsMap[actorId], reactionTime, actorId.ToString());
    }

    public static void RunReactiveStates(Dictionary<uint, List<(ABaseState, DateTime)>> idToStatesMap,
        List<Actor> actors)
    {
        // FIXME: Generalize, perhaps by adding extra parameter and intiializing ReactingActorsMap?
        foreach (var actorStatePair in idToStatesMap)
        {
            var seminarGroupId = -1;

            foreach (var stateTimePair in actorStatePair.Value)
            {
                switch (stateTimePair.Item1.Resource.Name)
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
        foreach (var actorStatesPair in idToStatesMap)
        {
            foreach (var stateTimePair in actorStatesPair.Value)
            {
                foreach (var state in ReactiveStates)
                {
                    if (state.ReactToActivity == stateTimePair.Item1.ActivityType)
                    {
                        if (state.ReactToResourceName != null && stateTimePair.Item1.Resource.Name != state.ReactToResourceName)
                        {
                            continue;
                        }
                        
                        // Assign same resource to the reacting state
                        state.Resource = state.OwnResource ?? stateTimePair.Item1.Resource;
                        var variableDirection = RandomService.GetNext(2) == 0 ? 1 : -1;
                        var variableTime = TimeSpan.FromTicks((long)(RandomService.GetNextDouble() * (state.TimeVariable.Ticks) * variableDirection));
                        var reactionTime = stateTimePair.Item2 + state.Offset + variableTime;
                        
                        AddReactiveState(state, reactionTime, actorStatesPair.Key);
                    }
                }
            }
        }
        
        // Execute reactive scenario
        foreach (var actorStatesPair in idToStatesMap)
        {
            foreach (var scenario in ReactiveScenarios)
            {
                var stateTimePairs = actorStatesPair.Value;

                int scenarioIndex = 0;
                for (int i = 0; i < stateTimePairs.Count; i++)
                {
                    var stateTimePair = stateTimePairs[i];

                    if (stateTimePair.Item1.ActivityType == scenario.MatchingPattern[scenarioIndex])
                    {
                        ++scenarioIndex;
                    }
                    else
                    {
                        scenarioIndex = 0;
                    }

                    // found the scenario to be matching
                    if (scenarioIndex == scenario.MatchingPattern.Count)
                    {
                        int firstMatchedIndex = i - (scenarioIndex - 1);

                        for (int j = firstMatchedIndex; j < firstMatchedIndex + scenario.MatchingPattern.Count; j++)
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

    public static void LoadReactiveState(ReactiveState reactiveState)
    {
        ReactiveStates.Add(reactiveState);
    }

    public static void LoadReactiveScenario(ReactiveScenario reactiveScenario)
    {
        ReactiveScenarios.Add(reactiveScenario);
    }

    public static void ResetService()
    {
        ReactiveStates = new();
        ReactingActorsMap = new();
    }
}
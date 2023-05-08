using EventLogGenerator.Services;

namespace EventLogGenerator.Models.States;

public class ScenarioSetSprinkle
{
    public HashSet<ProcessState> StartStates;

    public HashSet<ProcessState> EndStates;

    public List<(ABaseState, TimeSpan)> ScenarioSprinkleOffsets;

    public float ChanceToOccur;

    public ScenarioSetSprinkle(HashSet<ProcessState> startStates, HashSet<ProcessState> endStates,
        List<(ABaseState, TimeSpan)> scenarioSprinkleOffsets, float chanceToOccur)
    {
        if (!startStates.Any() || !endStates.Any())
        {
            throw new ArgumentException("Begin and ending states must be filled");
        }
        
        StartStates = startStates;
        EndStates = endStates;
        ScenarioSprinkleOffsets = scenarioSprinkleOffsets;
        ChanceToOccur = chanceToOccur;

        SprinkleService.LoadScenarioSprinkle(this);
    }
}
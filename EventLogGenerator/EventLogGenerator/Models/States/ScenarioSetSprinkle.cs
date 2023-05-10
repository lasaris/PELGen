using EventLogGenerator.Services;

namespace EventLogGenerator.Models.States;

public class ScenarioSetSprinkle
{
    public HashSet<ProcessState> StartStates;

    public HashSet<ProcessState> EndStates;

    // Matches scenario state to its minimum and maximum offset from previous state
    public List<(ABaseState, TimeSpan, TimeSpan)> ScenarioSprinkleOffsets;

    public float ChanceToOccur;

    public ScenarioSetSprinkle(HashSet<ProcessState> startStates, HashSet<ProcessState> endStates,
        List<(ABaseState, TimeSpan, TimeSpan)> scenarioSprinkleOffsets, float chanceToOccur)
    {
        if (!startStates.Any() || !endStates.Any() || !scenarioSprinkleOffsets.Any())
        {
            throw new ArgumentException("Begin, end and scenario itself must be filled");
        }
        
        StartStates = startStates;
        EndStates = endStates;
        ScenarioSprinkleOffsets = scenarioSprinkleOffsets;
        ChanceToOccur = chanceToOccur;

        SprinkleService.LoadScenarioSprinkle(this);
    }
}
namespace EventLogGenerator.Models;

public class StateChances
{
    // Chance of looping back to this state. If looping is not allowed, should be set to 0
    public float LoopChance;

    // Chance to finish on this state before reaching the end of the process
    public float FinishChance;

    public StateChances(float loopChance = 0.3f, float finishChance = 0f)
    {
        LoopChance = loopChance;
        FinishChance = finishChance;
    }
}
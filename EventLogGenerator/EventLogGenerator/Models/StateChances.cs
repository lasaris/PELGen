using EventLogGenerator.GenerationLogic;

namespace EventLogGenerator.Models;

public class StateChances
{
    // Chance to visit current state. If state is compulsory, it overrides this setting to 1
    public float ChanceToVisit;
    
    // Chance of looping back to this state. If looping is not allowed, should be set to 0
    public float LoopChance;

    // Chance to finish on this state before reaching the end of the process
    public float FinishChance;

    public StateChances(float chanceToVisit = Constants.ChanceToVisit, float loopChance = Constants.DefaultLoopChance, float finishChance = Constants.DeaultFinishChance)
    {
        ChanceToVisit = chanceToVisit;
        LoopChance = loopChance;
        FinishChance = finishChance;
    }
}
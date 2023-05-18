using EventLogGenerator.Models.States;

namespace EventLogGenerator.Models;

public abstract class ABaseRule
{
    public ABaseState Checkpoint;

    public (ABaseState, DateTime) NegativeEnd;

    protected ABaseRule(ABaseState checkpoint, (ABaseState, DateTime) negativeEnd)
    {
        Checkpoint = checkpoint;
        NegativeEnd = negativeEnd;
    }

    public abstract bool Evaluate(List<(ABaseState, DateTime, string?)> processStates);

    public (ABaseState, DateTime) GetEnd()
    {
        return NegativeEnd;
    }
}
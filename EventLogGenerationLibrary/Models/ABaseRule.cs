using EventLogGenerationLibrary.Models.States;

namespace EventLogGenerationLibrary.Models;

/// <summary>
/// Used to construct rules, that are checked, when the process is finished generating, right before logging it.
/// Checkpoint specifies at which state the rule should be checked and if Evaluate returns false, then NegativeEnd
/// is added to the process after the checkpoint (rest of the process is cut off).
/// </summary>
public abstract class ABaseRule
{
    public ABaseState Checkpoint;

    public (ABaseState, DateTime) NegativeEnd;

    protected ABaseRule(ABaseState checkpoint, (ABaseState, DateTime) negativeEnd)
    {
        Checkpoint = checkpoint;
        NegativeEnd = negativeEnd;
    }

    internal abstract bool Evaluate(OrderedTrace trace);
}
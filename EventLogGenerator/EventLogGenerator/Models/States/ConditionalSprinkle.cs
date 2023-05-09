using EventLogGenerator.Services;

namespace EventLogGenerator.Models.States;

public class ConditionalSprinkle
{
    public ProcessState StateToOccur;

    public DummyState PositiveState;

    public DummyState NegativeState;

    public TimeSpan TimeOffset;

    public ConditionalSprinkle(ProcessState stateToOccur, DummyState positiveState, DummyState negativeState,
        TimeSpan timeOffset)
    {
        StateToOccur = stateToOccur;
        PositiveState = positiveState;
        NegativeState = negativeState;
        TimeOffset = timeOffset;

        SprinkleService.LoadConditionalSprinkle(this);
    }
}
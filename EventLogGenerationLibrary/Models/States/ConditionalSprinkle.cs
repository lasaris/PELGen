﻿using EventLogGenerationLibrary.Services;
using EventLogGenerator.Models;
using EventLogGenerator.Services;

namespace EventLogGenerationLibrary.Models.States;

/// <summary>
/// It checks whether the StateToOccur occurs in the trace or not. If yes, then adds PositiveState to the process.
/// Otherwise it tries to guess the time which the ProcessState could've occured and adds NegativeState to the process.
/// When any DummyState is added, attribute TimeOffset is used and added to the time of StateToOccur. 
/// </summary>
public class ConditionalSprinkle
{
    // The desired state we are looking for
    public ProcessState StateToOccur;

    // The state added when StateToOccur is found
    public DummyState PositiveState;

    // The state added when StateToOccur did not occur
    public DummyState? NegativeState;

    // Fixed offset from the StateToOccur time
    public TimeSpan TimeOffset;

    public ConditionalSprinkle(ProcessState stateToOccur, DummyState positiveState, DummyState? negativeState,
        TimeSpan timeOffset)
    {
        StateToOccur = stateToOccur;
        PositiveState = positiveState;
        NegativeState = negativeState;
        TimeOffset = timeOffset;

        SprinkleService.LoadConditionalSprinkle(this);
    }
}
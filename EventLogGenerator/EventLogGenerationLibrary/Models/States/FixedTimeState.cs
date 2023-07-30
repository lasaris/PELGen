﻿using EventLogGenerationLibrary.Services;
using EventLogGenerator.Models;
using EventLogGenerator.Services;

namespace EventLogGenerationLibrary.Models.States;

/// <summary>
/// This state is always added to the process in a fixed time.
/// MustOccur is an optional attribute that serves as an abstract condition, so that this state is only added
/// when that state is present in the process (regardless of the time added).
/// </summary>
internal class FixedTimeState : ABaseState
{
    // Time at which state will be visited
    internal DateTime VisitTime;

    // State that must occur
    internal ProcessState? MustOccur;

    internal FixedTimeState(string activityType, string resource, DateTime visitTime,
        ProcessState? mustOccur = null) : base(activityType, resource)
    {
        VisitTime = visitTime;
        MustOccur = mustOccur;
        
        FixedTimeStateService.LoadFixedState(this);
    }
}
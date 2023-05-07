using EventLogGenerator.Models;
using EventLogGenerator.Models.Enums;
using EventLogGenerator.Models.States;

namespace EventLogGenerator.Services;

public class FixedTimeState : ABaseState
{
    // Time at which state will be visited
    public DateTime VisitTime;

    // State that must precede
    public ProcessState? MustPrecede;

    public FixedTimeState(EActivityType activityType, Resource resource, DateTime visitTime,
        ProcessState? mustPrecede = null) : base(activityType, resource)
    {
        VisitTime = visitTime;
        MustPrecede = mustPrecede;
        
        FixedTimeStateService.LoadFixedState(this);
    }
}
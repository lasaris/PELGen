using EventLogGenerator.Models;
using EventLogGenerator.Models.Enums;

namespace EventLogGenerator.Services;

public class FixedTimeState : ABaseState
{
    // Time at which state will be visited
    public DateTime VisitTime;

    public FixedTimeState(EActivityType activityType, Resource resource, DateTime visitTime) : base(activityType, resource)
    {
        VisitTime = visitTime;

        FixedTimeStateService.LoadFixedState(this);
    }
}
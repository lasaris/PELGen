using EventLogGenerator.Models.Enums;

namespace EventLogGenerator.Models.States;

public class DummyState : ABaseState
{
    public DummyState(EActivityType activityType, Resource resource) : base(activityType, resource)
    {
    }
}
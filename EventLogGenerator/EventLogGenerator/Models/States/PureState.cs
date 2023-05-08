using EventLogGenerator.Models.Enums;

namespace EventLogGenerator.Models.States;

public class PureState : ABaseState
{
    public PureState(EActivityType activityType, Resource resource) : base(activityType, resource)
    {
    }
}
using EventLogGenerator.Models.Enums;
using EventLogGenerator.Services;

namespace EventLogGenerator.Models;

public class ReactiveState : ABaseState
{
    // Activity that should be reacted to
    public EActivityType ReactTo;
    
    public ReactiveState(EActivityType activityType, Resource resource, EActivityType reactTo) :
        base(activityType, resource)
    {
        ReactTo = reactTo;

        ReactiveStateService.LoadFixedState(this);
    }
}
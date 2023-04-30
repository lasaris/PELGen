using EventLogGenerator.Models.Enums;
using EventLogGenerator.Services;

namespace EventLogGenerator.Models;

public class ReactiveState : ABaseState
{
    // The event that should be reacted to and should be mirrored with the same time
    public ABaseState ReactTo;

    // Actor that reacts to other state
    public Actor ReactingActor;

    public ReactiveState(EActivityType activityType, Resource resource, ABaseState reactTo, Actor reactingActor) : base(activityType, resource)
    {
        ReactTo = reactTo;
        ReactingActor = reactingActor;

        ReactiveStateService.LoadFixedState(this);
    }
}
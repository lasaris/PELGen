using EventLogGenerator.Models.Enums;
using EventLogGenerator.Models.States;
using EventLogGenerator.Services;

namespace EventLogGenerator.Models;

public class ReactiveState : ABaseState
{
    // Activity that should be reacted to
    public EActivityType ReactToActivity;

    public string? ReactToResourceName;

    public Resource? OwnResource;

    public TimeSpan Offset;

    public ReactiveState(EActivityType activityType, Resource resource, EActivityType reactToActivity,
        string? reactToResourceName = null, Resource? ownResource = null, TimeSpan? offset = null) : base(activityType, resource)
    {
        ReactToActivity = reactToActivity;
        ReactToResourceName = reactToResourceName;
        OwnResource = ownResource;
        Offset = offset ?? TimeSpan.Zero;

        ReactiveStateService.LoadFixedState(this);
    }
}
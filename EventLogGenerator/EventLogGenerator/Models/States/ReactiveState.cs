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

    public TimeSpan TimeVariable;

    public ReactiveState(EActivityType activityType, Resource resource, EActivityType reactToActivity,
        string? reactToResourceName = null, Resource? ownResource = null, TimeSpan? offset = null,
        TimeSpan? variable = null) : base(activityType, resource)
    {
        ReactToActivity = reactToActivity;
        ReactToResourceName = reactToResourceName;
        OwnResource = ownResource;
        Offset = offset ?? TimeSpan.Zero;
        TimeVariable = variable ?? TimeSpan.Zero;

        ReactiveStateService.LoadReactiveState(this);
    }
}
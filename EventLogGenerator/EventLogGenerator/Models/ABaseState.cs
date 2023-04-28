using EventLogGenerator.Models.Enums;

namespace EventLogGenerator.Models;

public class ABaseState
{
    // Activity to be performed
    public EActivityType ActivityType;

    // Resource to be performed with
    public Resource Resource;

    protected ABaseState(EActivityType activityType, Resource resource)
    {
        ActivityType = activityType;
        Resource = resource;
    }
}
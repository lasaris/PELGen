namespace EventLogGenerationLibrary.Models.States;

/// <summary>
/// Abstract class representing a state (an event) in the process, with the necessary attributes.
/// </summary>
public abstract class ABaseState
{
    // Activity to be performed
    public string ActivityType;

    // Resource to be performed with
    public string Resource;

    protected ABaseState(string activityType, string resource)
    {
        ActivityType = activityType;
        Resource = resource;
    }
}
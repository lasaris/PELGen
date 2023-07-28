namespace EventLogGenerationLibrary.Models.States;

/// <summary>
/// Abstract class representing a state (an event) in the process, with the necessary attributes.
/// </summary>
internal abstract class ABaseState
{
    // Activity to be performed
    internal string ActivityType;

    // Resource to be performed with
    internal string Resource;

    protected ABaseState(string activityType, string resource)
    {
        ActivityType = activityType;
        Resource = resource;
    }
}
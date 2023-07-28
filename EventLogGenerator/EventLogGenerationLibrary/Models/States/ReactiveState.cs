using EventLogGenerationLibrary.Services;
using EventLogGenerator.Services;

namespace EventLogGenerationLibrary.Models.States;

/// <summary>
/// State that reacts to one specific activity in the process.
/// When ReactToActivity is found, it adds Offset to its time and then with TimeVariable randomly selects timestamp.
/// Optionally ReactoToResoruceName can be specified to additionally also check for resource.
/// Optionally OwnResource can be defined to specify what other resource should be associated with the created reaction. 
/// </summary>
internal class ReactiveState : ABaseState
{
    // Activity that should be reacted to
    internal string ReactToActivity;

    // Optionally specified if we want to match also resource with the activity
    internal string? ReactToResourceName;
    
    // Optionally specified if we want to visit this state with our own resource
    internal string? OwnResource;

    // Specifies the amount of time offset from the reacting state
    internal TimeSpan Offset;

    // Specifies the variability around the timestamp of current state (+ optionally added offset)
    internal TimeSpan TimeVariable;

    internal ReactiveState(string activityType, string resource, string reactToActivity,
        string? reactToResourceName = null, string? ownResource = null, TimeSpan? offset = null,
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
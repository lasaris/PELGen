namespace EventLogGenerationLibrary.Models.States;

/// <summary>
/// Can be instantiated as the most basic state creatable for the process event.
/// Used when no process state or other type can be used and by itself is not added any way to the process.
/// </summary>
internal class DummyState : ABaseState
{
    internal DummyState(string activityType, string resource) : base(activityType, resource)
    {
    }
}
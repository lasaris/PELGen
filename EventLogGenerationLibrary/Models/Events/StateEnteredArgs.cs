using EventLogGenerationLibrary.Models.States;

namespace EventLogGenerationLibrary.Models.Events;

/// <summary>
/// Used as an event to transfer information that (any type of) state was visited in the process.
/// </summary>
internal class StateEnteredArgs : AStateArgs
{
    internal StateEnteredArgs(ABaseState state, Actor actor, DateTime timeStamp, string? additional = null) : base(state,
        actor, timeStamp, additional)
    {
    }
}
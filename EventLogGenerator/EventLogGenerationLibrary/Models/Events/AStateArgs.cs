using EventLogGenerationLibrary.Models.States;

namespace EventLogGenerationLibrary.Models.Events;

internal abstract class AStateArgs : EventArgs
{
    internal ABaseState State;

    internal Actor Actor;

    internal DateTime TimeStamp;

    internal string? Additional;

    protected AStateArgs(ABaseState state, Actor actor, DateTime timeStamp, string? additional = null)
    {
        State = state;
        Actor = actor;
        TimeStamp = timeStamp;
        Additional = additional;
    }
}
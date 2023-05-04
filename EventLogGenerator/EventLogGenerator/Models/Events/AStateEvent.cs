using EventLogGenerator.Models.States;

namespace EventLogGenerator.Models.Events;

public abstract class AStateEvent : EventArgs
{
    public ABaseState State;

    public Actor Actor;

    public DateTime TimeStamp;

    public string? Additional;

    protected AStateEvent(ABaseState state, Actor actor, DateTime timeStamp, string? additional = null)
    {
        State = state;
        Actor = actor;
        TimeStamp = timeStamp;
        Additional = additional;
    }
}
namespace EventLogGenerator.Models;

public abstract class AStateEvent : EventArgs
{
    public ABaseState State;

    public Actor Actor;

    public DateTime TimeStamp;

    protected AStateEvent(ABaseState state, Actor actor, DateTime timeStamp)
    {
        State = state;
        Actor = actor;
        TimeStamp = timeStamp;
    }
}
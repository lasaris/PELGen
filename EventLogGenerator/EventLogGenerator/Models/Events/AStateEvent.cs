namespace EventLogGenerator.Models;

public abstract class AStateEvent : EventArgs
{
    public ProcessState State;

    public Actor Actor;

    public DateTime TimeStamp;

    protected AStateEvent(ProcessState state, Actor actor, DateTime timeStamp)
    {
        State = state;
        Actor = actor;
        TimeStamp = timeStamp;
    }
}
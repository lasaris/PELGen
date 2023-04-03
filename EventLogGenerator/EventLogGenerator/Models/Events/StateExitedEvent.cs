namespace EventLogGenerator.Models;

public class StateExitedEvent : AStateEvent
{
    public StateExitedEvent(ProcessState state, Actor actor, DateTime timeStamp) : base(state, actor, timeStamp)
    {
    }
}
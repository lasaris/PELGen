namespace EventLogGenerator.Models;

public class StateEnteredEvent : AStateEvent
{
    public StateEnteredEvent(ProcessState state, Actor actor, DateTime timeStamp) : base(state, actor, timeStamp)
    {
    }
}
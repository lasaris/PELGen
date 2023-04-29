namespace EventLogGenerator.Models;

public class StateEnteredEvent : AStateEvent
{
    public StateEnteredEvent(ABaseState state, Actor actor, DateTime timeStamp) : base(state, actor, timeStamp)
    {
    }
}
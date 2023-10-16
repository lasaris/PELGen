using EventLogGenerator.Models.States;

namespace EventLogGenerator.Models.Events;

public class StateEnteredEvent : AStateEvent
{
    public StateEnteredEvent(ABaseState state, Actor actor, DateTime timeStamp, string? additional = null) : base(state,
        actor, timeStamp, additional)
    {
    }
}
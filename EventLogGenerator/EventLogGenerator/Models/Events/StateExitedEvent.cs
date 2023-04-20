namespace EventLogGenerator.Models;

public class StateExitedEvent : AStateEvent
{
    // FIXME: This is never used and AStateEvent should be refactored
    public StateExitedEvent(ProcessState state, Actor actor, DateTime timeStamp) : base(state, actor, timeStamp)
    {
    }
}
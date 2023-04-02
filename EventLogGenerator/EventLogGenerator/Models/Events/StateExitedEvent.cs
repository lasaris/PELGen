namespace EventLogGenerator.Models;

public class StateExitedEvent : AStateEvent
{
    public StateExitedEvent(ProcessState state, DateTime timeStamp) : base(state, timeStamp)
    {
    }
}
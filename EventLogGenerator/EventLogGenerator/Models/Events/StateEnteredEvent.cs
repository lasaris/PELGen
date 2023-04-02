namespace EventLogGenerator.Models;

public class StateEnteredEvent : AStateEvent
{
    public StateEnteredEvent(ProcessState state, DateTime timeStamp) : base(state, timeStamp)
    {
    }
}
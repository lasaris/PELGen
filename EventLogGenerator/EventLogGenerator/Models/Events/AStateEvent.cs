namespace EventLogGenerator.Models;

public abstract class AStateEvent
{
    public ProcessState State;

    public DateTime TimeStamp;

    protected AStateEvent(ProcessState state, DateTime timeStamp)
    {
        State = state;
        TimeStamp = timeStamp;
    }
}
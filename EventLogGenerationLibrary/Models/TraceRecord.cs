using EventLogGenerationLibrary.Models.States;

namespace EventLogGenerationLibrary.Models;

public class TraceRecord
{
    public ABaseState State;

    public DateTime Time;

    public string? Additional;

    public TraceRecord(ABaseState state, DateTime time, string? additional)
    {
        State = state;
        Time = time;
        Additional = additional;
    }
}
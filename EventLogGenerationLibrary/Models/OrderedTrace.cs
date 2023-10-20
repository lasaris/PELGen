using System.Text.Json.Serialization;
using EventLogGenerationLibrary.Models.States;

namespace EventLogGenerationLibrary.Models;

/// <summary>
/// Represents an ordered collection of events (a single trace).
/// By default the events are kept in chronological order.
/// </summary>
public class OrderedTrace
{
    // Collection to keep traces
    public List<TraceRecord> Trace;

    // Last added timestamp, auxiliary attribute for keeping chronological order
    private DateTime _lastTime;

    public OrderedTrace()
    {
        Trace = new List<TraceRecord>();
        _lastTime = DateTime.MinValue;
    }

    internal OrderedTrace(List<TraceRecord> trace)
    {
        Trace = trace;
        _lastTime = DateTime.MinValue;
    }

    internal void Add(TraceRecord record)
    {
        Trace.Add(record);

        if (record.Time < _lastTime)
        {
            Trace = Trace.OrderBy(item => item.Time).ToList();
        }

        _lastTime = record.Time;
    }
    
    public TraceRecord this[int index] => Trace[index];
}
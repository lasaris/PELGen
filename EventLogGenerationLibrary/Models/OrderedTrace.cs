using EventLogGenerationLibrary.Models.States;

namespace EventLogGenerationLibrary.Models;

/// <summary>
/// Represents an ordered collection of events (a single trace).
/// By default the events are kept in chronological order.
/// </summary>
internal class OrderedTrace
{
    // Collection to keep traces
    internal List<(ABaseState, DateTime, string?)> Trace;

    // Last added timestamp, auxiliary attribute for keeping chronological order
    private DateTime _lastTime;

    public OrderedTrace()
    {
        Trace = new List<(ABaseState, DateTime, string?)>();
        _lastTime = DateTime.MinValue;
    }

    internal OrderedTrace(List<(ABaseState, DateTime, string?)> trace)
    {
        Trace = trace;
        _lastTime = DateTime.MinValue;
    }

    internal void Add((ABaseState, DateTime, string?) action)
    {
        Trace.Add(action);

        if (action.Item2 < _lastTime)
        {
            Trace = Trace.OrderBy(item => item.Item2).ToList();
        }

        _lastTime = action.Item2;
    }
    
    public (ABaseState, DateTime, string?) this[int index] => Trace[index];
}
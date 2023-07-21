using EventLogGenerator.Models.States;

namespace EventLogGenerator.Models;

public class OrderedTrace
{
    public List<(ABaseState, DateTime, string?)> Trace;

    private DateTime _lastTime;

    public OrderedTrace(List<(ABaseState, DateTime, string?)> trace)
    {
        Trace = trace;
        _lastTime = DateTime.MinValue;
    }

    public void Add((ABaseState, DateTime, string?) action)
    {
        Trace.Add(action);

        if (action.Item2 < _lastTime)
        {
            Trace = Trace.OrderBy(item => item.Item2).ToList();
        }

        _lastTime = action.Item2;
    }
}
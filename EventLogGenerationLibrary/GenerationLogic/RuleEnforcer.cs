using EventLogGenerationLibrary.Models;
using EventLogGenerationLibrary.Models.States;

namespace EventLogGenerationLibrary.GenerationLogic;

/// <summary>
/// Collects created rules and evaluates traces based on them.
/// </summary>
internal static class RuleEnforcer
{
    internal static HashSet<ABaseRule> Rules = new();
    
    internal static void LoadRule(ABaseRule newRule)
    {
        Rules.Add(newRule);
    }

    // Run registered rules on a process and returns it after it was processed
    internal static OrderedTrace GetEvaluatedTrace(OrderedTrace trace)
    {
        if (!Rules.Any())
        {
            return trace;
        }
        
        var orderedTrace = new OrderedTrace(trace.Trace.OrderBy(record => record.Time).ToList());
        var newProcess = new OrderedTrace();
        foreach (var rule in Rules)
        {
            // Rule evaluated positively (no need to change process)
            if (rule.Evaluate(orderedTrace))
            {
                continue;
            }
            
            foreach (var record in orderedTrace.Trace)
            {
                if (record.State == rule.Checkpoint)
                {
                    newProcess.Add(new TraceRecord(rule.NegativeEnd.Item1, rule.NegativeEnd.Item2, null));
                    break;
                }
                newProcess.Add(record);
            }

            break;
        }

        return (newProcess.Trace.Any()) ? newProcess : trace;
    }

    internal static void ResetService()
    {
        Rules = new();
    }
}
using EventLogGenerator.Models;
using EventLogGenerator.Models.States;

namespace EventLogGenerator.GenerationLogic;

public static class RuleEnforcer
{
    public static HashSet<ABaseRule> Rules = new();
    
    public static void LoadRule(MinimumActivityCountRule newRule)
    {
        Rules.Add(newRule);
    }

    // Run registered rules on a process and returns it after it was processed
    public static List<(ABaseState, DateTime, string?)> GetEvaluatedProcess(List<(ABaseState, DateTime, string?)> process)
    {
        if (!Rules.Any())
        {
            return process;
        }
        
        var orderedProcess = process.OrderBy(item => item.Item2).ToList();
        var newProcess = new List<(ABaseState, DateTime, string?)>();
        foreach (var rule in Rules)
        {
            // Rule evaluated positively (no need to change process)
            if (rule.Evaluate(orderedProcess))
            {
                continue;
            }
            
            foreach (var state in orderedProcess)
            {
                if (state.Item1 == rule.Checkpoint)
                {
                    newProcess.Add((rule.NegativeEnd.Item1, rule.NegativeEnd.Item2, null));
                    break;
                }
                newProcess.Add(state);
            }

            break;
        }

        return (newProcess.Any()) ? newProcess : process;
    }

    public static void ResetService()
    {
        Rules = new();
    }
}
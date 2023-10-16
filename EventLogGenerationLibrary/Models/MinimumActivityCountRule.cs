using EventLogGenerationLibrary.GenerationLogic;
using EventLogGenerationLibrary.Models.States;

namespace EventLogGenerationLibrary.Models;

/// <summary>
/// Rule that specifies the minimum count of events with an Activity that must be present in the process.
/// </summary>
public class MinimumActivityCountRule : ABaseRule
{
    public string Activity;

    public readonly int MinimumCount;

    public MinimumActivityCountRule(ABaseState checkpoint, (ABaseState, DateTime) negativeEnd, string activity,
        int minimumCount) : base(checkpoint, negativeEnd)
    {
        Activity = activity;
        MinimumCount = minimumCount;

        RuleEnforcer.LoadRule(this);
    }
    
    internal override bool Evaluate(OrderedTrace trace)
    {
        var currentCount = 0;
        foreach(var state in trace.Trace)
        {
            if (state.Item1 == Checkpoint)
            {
                break;
            }
            
            if (state.Item1.ActivityType == Activity)
            {
                currentCount++;
            }
        }

        return currentCount >= MinimumCount;
    }
}
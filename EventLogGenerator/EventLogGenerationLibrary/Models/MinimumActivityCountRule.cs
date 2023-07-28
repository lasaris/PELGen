using EventLogGenerationLibrary.GenerationLogic;
using EventLogGenerationLibrary.Models.States;

namespace EventLogGenerationLibrary.Models;

internal class MinimumActivityCountRule : ABaseRule
{
    internal string Activity;

    internal readonly int MinimumCount;

    internal MinimumActivityCountRule(ABaseState checkpoint, (ABaseState, DateTime) negativeEnd, string activity,
        int minimumCount) : base(checkpoint, negativeEnd)
    {
        Activity = activity;
        MinimumCount = minimumCount;

        RuleEnforcer.LoadRule(this);
    }
    
    internal override bool Evaluate(List<(ABaseState, DateTime, string?)> process)
    {
        var currentCount = 0;
        foreach(var state in process)
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
using EventLogGenerator.GenerationLogic;
using EventLogGenerator.Models.Enums;
using EventLogGenerator.Models.States;

namespace EventLogGenerator.Models;

public class MinimumActivityCountRule : ABaseRule
{
    public EActivityType Activity;

    public readonly int MinimumCount;


    public MinimumActivityCountRule(ABaseState checkpoint, (ABaseState, DateTime) negativeEnd, EActivityType activity,
        int minimumCount) : base(checkpoint, negativeEnd)
    {
        Activity = activity;
        MinimumCount = minimumCount;

        RuleEnforcer.LoadRule(this);
    }
    
    public override bool Evaluate(List<(ABaseState, DateTime, string)> process)
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
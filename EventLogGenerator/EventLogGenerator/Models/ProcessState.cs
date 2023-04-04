using EventLogGenerator.GenerationLogic;
using EventLogGenerator.Models.Enums;

namespace EventLogGenerator.Models;

public class ProcessState
{
    // Activity for given state
    public EActivityType ActivityType;
    
    // Resource used to accomplish activity of the state
    public Resource Resource;

    // Rules which apply for given state and which lead to the next
    public StateRules Rules;

    // Chances properties for the upcoming move decisions
    public StateChances Chances;

    // Start and end time in between which the state can be visited
    public TimeFrame TimeFrame;

    // Indicates if this is the starting state of process (should be only one)
    public bool IsStarting;

    // Indicates if process is finished with this state (can be multiple states)
    public bool IsFinishing;

    public ProcessState(EActivityType activity, Resource resource, StateRules rules, StateChances chances, TimeFrame timeFrame, bool isStarting = false, bool isFinishing = false)
    {
        ActivityType = activity;
        Resource = resource;
        Rules = rules;
        
        // Override compulsory states chance for visit to 100%
        if (rules.IsCompulsory)
        {
            chances.ChanceToVisit = 1f;
        }
        
        Chances = chances;
        TimeFrame = timeFrame;
        IsStarting = isStarting;
        IsFinishing = isFinishing;

        StateEvaluator.AddState(this);
    }
}
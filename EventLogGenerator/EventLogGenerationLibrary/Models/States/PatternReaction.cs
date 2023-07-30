using EventLogGenerationLibrary.Services;
using EventLogGenerator.Services;

namespace EventLogGenerationLibrary.Models.States;

/// <summary>
/// 
/// </summary>
public class PatternReaction
{
    // Pattern of activities to match in the process
    public List<string> ActivitiesPattern;

    // NOTE: Could be specified better in case 2 activities match in the pattern, perhaps use index instead?
    // Activity to match time with
    public string MatchTimeWith;

    // Activity that should be reacted with
    public string Reaction;

    public PatternReaction(List<string> activitiesPattern, string matchTimeWith, string reaction)
    {
        if (!activitiesPattern.Any())
        {
            throw new ArgumentException("Reactive scenario must have pattern of states that it can react to");
        }
        
        ActivitiesPattern = activitiesPattern;
        MatchTimeWith = matchTimeWith;
        Reaction = reaction;

        ReactiveStateService.LoadReactiveScenario(this);
    }
}
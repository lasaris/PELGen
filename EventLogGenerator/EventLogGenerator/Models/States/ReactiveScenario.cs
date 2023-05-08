using EventLogGenerator.Models.Enums;
using EventLogGenerator.Services;

namespace EventLogGenerator.Models.States;

public class ReactiveScenario
{
    public List<EActivityType> MatchingPattern;

    public EActivityType MatchTimeWith;

    public EActivityType Reaction;

    public ReactiveScenario(List<EActivityType> matchingPattern, EActivityType matchTimeWith, EActivityType reaction)
    {
        if (!matchingPattern.Any())
        {
            throw new ArgumentException("Reactive scenario must have pattern of states that it can react to");
        }
        
        MatchingPattern = matchingPattern;
        MatchTimeWith = matchTimeWith;
        Reaction = reaction;

        ReactiveStateService.LoadReactiveScenario(this);
    }
}
using EventLogGenerationLibrary.GenerationLogic;
using EventLogGenerator.Services;

namespace EventLogGenerationLibrary.Models.Modifiers;

/// <summary>
/// Modifies (interval) state so that there is additional information about actor ID also logged.
/// Can be specified how many times the state with random ID should occur.
/// </summary>
public class AdditionalRandomIdModifier
{
    /// <summary>
    /// The event actions to react and apply the modifier. If empty, modifier will be applied always"
    /// </summary>
    public List<string> ActionsToReact;

    /// <summary>
    /// How many times should the state be repeated with random actors ID. Default is 1.
    /// </summary>
    public int NumberOfOccurrences;

    public string GetRandomActorId()
    {
        var totalNumberOfActors = (int)(Collector.GetLastCollectionMaxId() + 1 - IdService.InitialSetId);
        return (RandomService.GetNext(totalNumberOfActors) + IdService.InitialSetId).ToString();
    }

    public AdditionalRandomIdModifier(List<string> actionsToReact, int? numberOfOccurrences = null,
        float lowerBound = 1 / 6f, float upperBound = 1 / 2f)
    {
        ActionsToReact = actionsToReact;
        if (numberOfOccurrences == null)
        {
            var totalNumberOfActors = (int)(Collector.GetLastCollectionMaxId() + 1 - IdService.InitialSetId);
            NumberOfOccurrences = (int)Math.Max(RandomService.GetNext((int)(totalNumberOfActors * upperBound)), totalNumberOfActors * lowerBound);
        }
        else
        {
            NumberOfOccurrences = (int)numberOfOccurrences;
        }
    }
}
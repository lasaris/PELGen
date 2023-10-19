using EventLogGenerationLibrary.Models;
using EventLogGenerationLibrary.Models.States;
using EventLogGenerator.Models;

namespace EventLogGenerationLibrary;

public class Configuration
{
    /// <summary>
    /// Number of actors (traces generated)
    /// </summary>
    public int ActorCount { get; set; }

    /// <summary>
    /// The beginning state where the process is initiated
    /// </summary>
    public ProcessState StartState { get; set; }

    /// <summary>
    /// Initial ID for Actors. They are generated automatically with ID incremented by 1.
    /// </summary>
    public uint? InitialId { get; set; }

    /// <summary>
    /// If you decide not to give Initial ID, you can also create configuration with custom Actors (and their IDs)
    /// </summary>
    public List<Actor>? Actors { get; set; }
    
    /// <summary>
    /// Some activities within the process can by limited globally (i.e. when looping or appearing multiple times)
    /// </summary>
    public Dictionary<string, int> ActivityLimits { get; set; }

    /// <summary>
    /// Name of the generated event log file. It is recommended to end with ".csv".
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// The header for generated output file.
    /// </summary>
    public string FileHeader { get; set; }

    /// <summary>
    /// Type of actor to be generated into the event log.
    /// </summary>
    public string ActorType { get; set; }

    /// <summary>
    /// If you want to enable reactive states, you need to specify to which process they should react to.
    /// </summary>
    public Process? ReactToProcess;

    /// <summary>
    /// When reacting, it is necessary to specify which actors should react to which previous actors.
    /// </summary>
    public ReactingActorStrategy? ReactionStrategy;

    public Configuration(int actorCount, ProcessState startState, uint initialId,
        Dictionary<string, int>? activityLimits, string fileName, string fileHeader, string actorType,
        Process? reactToProcess = null, ReactingActorStrategy? reactionStrategy = null)
    {
        ActorCount = actorCount;
        StartState = startState;
        InitialId = initialId;
        ActivityLimits = activityLimits ?? new Dictionary<string, int>();
        FileName = fileName;
        FileHeader = fileHeader;
        ActorType = actorType;
        ReactToProcess = reactToProcess;
        ReactionStrategy = reactionStrategy;
    }

    public Configuration(int actorCount, ProcessState startState, List<Actor> actors,
        Dictionary<string, int>? activityLimits, string fileName, string fileHeader, string actorType,
        Process? reactToProcess = null, ReactingActorStrategy? reactionStrategy = null)
    {
        ActorCount = actorCount;
        StartState = startState;
        Actors = actors;
        ActivityLimits = activityLimits ?? new Dictionary<string, int>();
        FileName = fileName;
        FileHeader = fileHeader;
        ActorType = actorType;
        ReactToProcess = reactToProcess;
        ReactionStrategy = reactionStrategy;
    }
}
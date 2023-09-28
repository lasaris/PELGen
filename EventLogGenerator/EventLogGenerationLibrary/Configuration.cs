using EventLogGenerationLibrary.Models;
using EventLogGenerationLibrary.Models.States;

namespace EventLogGenerationLibrary;

public class Configuration
{
    public int ActorCount { get; set; }

    public ProcessState StartState { get; set; }
    
    public uint? InitialId { get; set; }
    
    public List<Actor>? Actors { get; set; }
    
    public Dictionary<string, int> ActivityLimits { get; set; }
    
    public string FileName { get; set; }
    
    public string FileHeader { get; set; }
    
    public string ActorType { get; set; }

    public Configuration(int actorCount, ProcessState startState, uint initialId, Dictionary<string, int> activityLimits, string fileName, string fileHeader, string actorType)
    {
        ActorCount = actorCount;
        StartState = startState;
        InitialId = initialId;
        ActivityLimits = activityLimits;
        FileName = fileName;
        FileHeader = fileHeader;
        ActorType = actorType;
    }
    
    public Configuration(int actorCount, ProcessState startState, List<Actor> actors, Dictionary<string, int> activityLimits, string fileName, string fileHeader, string actorType)
    {
        ActorCount = actorCount;
        StartState = startState;
        Actors = actors;
        ActivityLimits = activityLimits;
        FileName = fileName;
        FileHeader = fileHeader;
        ActorType = actorType;
    }
}
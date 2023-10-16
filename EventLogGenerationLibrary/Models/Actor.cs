using EventLogGenerator.Services;

namespace EventLogGenerationLibrary.Models;

/// <summary>
/// Used for storing actor with unique Id and type specified.
/// </summary>
public class Actor
{
    // Unique ID
    public uint Id { get; set; }

    // Type of the Actor. Different Actor can have specific process generated
    public string Type;

    public Actor(string type, Dictionary<HashSet<string>, TimeSpan>? offsetMap = null)
    {
        Id = IdService.GetNewActorId();
        Type = type;
    }
}

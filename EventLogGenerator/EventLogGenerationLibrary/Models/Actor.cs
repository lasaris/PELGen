using EventLogGenerator.Services;

namespace EventLogGenerationLibrary.Models;

/// <summary>
/// Used for storing actor with unique Id and type specified.
/// </summary>
internal class Actor
{
    // Unique ID
    internal uint Id { get; set; }

    // Type of the Actor. Different Actor can have specific process generated
    internal string Type;

    internal Actor(string type, Dictionary<HashSet<string>, TimeSpan>? offsetMap = null)
    {
        Id = IdService.GetNewActorId();
        Type = type;
    }
}

using EventLogGenerator.Services;

namespace EventLogGenerationLibrary.Models;

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

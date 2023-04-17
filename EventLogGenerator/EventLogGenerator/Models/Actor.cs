using EventLogGenerator.Models.Enums;
using EventLogGenerator.Services;

namespace EventLogGenerator.Models;

public class Actor
{
    // Unique ID
    public uint Id { get; }

    // Type of the Actor. Different Actor can have specific process generated
    public EActorType Type;

    // Models time offset for given resource (i.e. student attending seminar a day or two later)
    public Dictionary<HashSet<Resource>, TimeSpan>? OffsetMap;

    public Actor(EActorType type, Dictionary<HashSet<Resource>, TimeSpan>? offsetMap = null)
    {
        Id = IdService.GetNewActorId();
        Type = type;
        OffsetMap = offsetMap;
    }
}

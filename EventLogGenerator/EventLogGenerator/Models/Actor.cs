using EventLogGenerator.Models.Enums;
using EventLogGenerator.Services;

namespace EventLogGenerator.Models;

public class Actor
{
    // Unique ID
    public uint Id;

    // Type of the Actor. Different Actor can have specific process generated
    public EActorType Type;

    public Actor(EActorType type)
    {
        Id = IdService.GetNewActorId();
        Type = type;
    }
}
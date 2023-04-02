using EventLogGenerator.Models.Enums;

namespace EventLogGenerator.Models;

public class Actor
{
    // Unique ID
    public int Id;

    // Type of the Actor. Different Actor can have specific process generated
    public EActorType Type;
}
using EventLogGenerator.Models.Enums;

namespace EventLogGenerator.Models;

/// <summary>
/// Represents Activity that Actor can perform
/// </summary>
public class Activity
{
    // Name of the Activity. Must be unique.
    public string Name;
    
    public Activity(EActivityType type)
    {
        Name = type.ToString() ?? "";
    }
}
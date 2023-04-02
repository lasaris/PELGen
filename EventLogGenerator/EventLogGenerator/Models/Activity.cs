namespace EventLogGenerator.Models;

/// <summary>
/// Represents Activity that Actor can perform
/// </summary>
public class Activity
{
    // Name of the Activity. Must be unique.
    public string Name;

    // Defines if Actor must perform this activity in the process
    public bool IsCompulsory;

    // Defines if Actor can return to this same Activity (create loop over it)
    public bool CanRepeat;

    // If actor can repeat the same activity, how many times can it loop
    public int MaxRepetitions;
    
    // How many times can this activity be performed in the process (-1 = unlimited times)
    public uint MaxPasses;

    // Chance to finish the process in this state
    public float ChanceToFinish;
    
    // Constructor with default values for each field
    public Activity(string name, bool isCompulsory = true, bool canRepeat = false, int maxRepetitions = 1, uint maxPasses = 1, float chanceToFinish = 0f)
    {
        Name = name;
        IsCompulsory = isCompulsory;
        CanRepeat = canRepeat;
        MaxRepetitions = maxRepetitions;
        MaxPasses = maxPasses;
        ChanceToFinish = chanceToFinish;
    }
}
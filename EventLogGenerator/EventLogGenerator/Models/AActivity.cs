namespace EventLogGenerator.Models;

/// <summary>
/// Represents Activity that Actor can perform
/// </summary>
public abstract class AActivity
{
    // Name of the Activity. Must be unique.
    public string Name = "";

    // Defines if Actor must perform this activity in the process
    public bool IsCompulsory = true;

    // Defines if Actor can return to this same Activity (create loop over it)
    public bool CanRepeat = false;

    // If actor can repeat the same activity, how many times can it loop
    public int MaxRepetitions = 1;
    
    // How many times can this activity be performed in the process (-1 = unlimited times)
    public uint MaxPasses = 1;

    // Chance to finish the process in this state
    public float ChanceToFinish = 0f;
}
using EventLogGenerator.Models;

namespace EventLogGenerator.Services;

/**
 * Sprinkles process with events from given checkpoints. These checkpoints can be performed anytime after some state.
 */
public static class SprinkleService
{
    // Sprinkles, that are yet to be sprinkled
    public static HashSet<SprinkleState> UpcomingSprinkles = new();

    // Sprinkles currently ready to be sprinkled into the process
    public static HashSet<SprinkleState> AvailableSprinkles = new();

    private static void UpdateAvailableSprinkles(ProcessState newState)
    {
        // Remove sprinkles with
        AvailableSprinkles = AvailableSprinkles.Where(state => state.StopBefore != newState).ToHashSet();

        // Add newly available sprinkles and remove them from upcoming
        var newlyAvailable = UpcomingSprinkles.Where(state => state.BeginAfter == newState);
        newlyAvailable.Select(state => AvailableSprinkles.Add(state));
        UpcomingSprinkles.ExceptWith(newlyAvailable.ToHashSet());
    }

    private static void AddRandomSprinkle(ProcessState newState)
    {
        // Weight each state
        var weightedStates = new Dictionary<SprinkleState, float>();
        foreach (var state in AvailableSprinkles)
        {
            // TODO: Weight sprinkle states. Is there really a way? Only priority should have ChanceRightAfter, which needs to be detected via newState parameter
        }
    }

    private static SprinkleState SelectNextSprinkle(Dictionary<SprinkleState, float> weightedSprinkles)
    {
        throw new NotImplementedException();
    }

    public static void StateEnteredHandler(object sender, StateEnteredEvent data)
    {
        UpdateAvailableSprinkles(data.State);
        AddRandomSprinkle(data.State);
    }

    public static void ResetSprinklerState()
    {
        AvailableSprinkles = new();
        UpcomingSprinkles = new();
    }

    public static void LoadSprinklerState(SprinkleState newSprinkle)
    {
        UpcomingSprinkles.Add(newSprinkle);
    }
}
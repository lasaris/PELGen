namespace EventLogGenerator.Services;

/// <summary>
/// Stores last known ID for entities and generate new unique ones for each entity.
/// </summary>
public static class IdService
{
    private const uint BeginningId = 0;

    private static uint _lastActorId = 1;

    public static uint InitialSetId = 1;

    public static uint GetNewActorId()
    {
        return _lastActorId++;
    }

    public static void SetInitialId(uint id)
    {
        InitialSetId = id;
        _lastActorId = id;
    }

    public static void ResetService()
    {
        _lastActorId = BeginningId;
    }
}
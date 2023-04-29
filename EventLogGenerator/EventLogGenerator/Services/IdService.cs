namespace EventLogGenerator.Services;

/// <summary>
/// Stores last known ID for entities and generate new unique ones for each entity.
/// </summary>
public static class IdService
{
    private const uint BeginningId = 0;

    private static uint _lastActorId = 0;

    public static uint GetNewActorId()
    {
        _lastActorId++;
        return _lastActorId;
    }

    public static void ResetService()
    {
        _lastActorId = BeginningId;
    }
}
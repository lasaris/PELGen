namespace EventLogGenerator.Services;

/// <summary>
/// Stores last known ID for entities and generate new unique ones for each entity.
/// </summary>
internal static class IdService
{
    private const uint BeginningId = 0;

    private static uint _lastActorId = 1;

    internal static uint InitialSetId = 1;

    internal static uint GetNewActorId()
    {
        return _lastActorId++;
    }

    internal static void SetInitialId(uint id)
    {
        InitialSetId = id;
        _lastActorId = id;
    }

    internal static void ResetService()
    {
        _lastActorId = BeginningId;
    }
}
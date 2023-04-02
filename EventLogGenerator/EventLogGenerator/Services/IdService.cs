﻿namespace EventLogGenerator.Services;

/// <summary>
/// Stores last known ID for entities and generate new unique ones for each entity.
/// </summary>
public static class IdService
{
    private static uint _lastActorId = 0;

    private static uint _lastEventId = 0;
    
    public static uint GetNewActorId()
    {
        _lastActorId++;
        return _lastActorId;
    }

    public static uint GetNewEventId()
    {
        _lastEventId++;
        return _lastEventId;
    }
}
using EventLogGenerator.Models;
using EventLogGenerator.Models.States;

namespace EventLogGenerator.GenerationLogic;

public static class Collector
{
    public static int LastIndex = -1;
    
    public static List<Dictionary<uint, List<(ABaseState, DateTime)>>> CreatedLogs = new();

    public static void CreateCollectorMap()
    {
        var createdDictionary = new Dictionary<uint, List<(ABaseState, DateTime)>>();
        CreatedLogs.Add(createdDictionary);
        ++LastIndex;
    }

    public static Dictionary<uint, List<(ABaseState, DateTime)>> GetPreviousCollection()
    {
        if (!CreatedLogs.Any())
        {
            throw new ArgumentException("There is no previous collection of logs created");
        }

        return CreatedLogs[LastIndex - 1];
    }

    public static uint GetLastCollectionMaxId()
    {
        return CreatedLogs[LastIndex - 1].Keys.Max();
    }

    public static Dictionary<uint, List<(ABaseState, DateTime)>> GetProcessMap(int index)
    {
        if (index > LastIndex || index < 0)
        {
            throw new ArgumentException("Index out of bounds for CreatedLogs");
        }

        return CreatedLogs[index];
    }
    
    public static void AddLog(uint actorId, ABaseState state, DateTime timeStamp)
    {
        if (!CreatedLogs[LastIndex].ContainsKey(actorId))
        {
            CreatedLogs[LastIndex][actorId] = new();
        }
        
        CreatedLogs[LastIndex][actorId].Add((state, timeStamp));
    }
}
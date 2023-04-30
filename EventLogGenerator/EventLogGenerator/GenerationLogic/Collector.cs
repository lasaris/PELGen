using EventLogGenerator.Models;

namespace EventLogGenerator.GenerationLogic;

public static class Collector
{
    public static Dictionary<uint, List<(ABaseState, DateTime)>> CreatedLogs = new();

    public static void AddLog(uint actorId, ABaseState state, DateTime timeStamp)
    {
        if (!CreatedLogs.ContainsKey(actorId))
        {
            CreatedLogs[actorId] = new();
        }
        
        CreatedLogs[actorId].Add((state, timeStamp));
    }
}
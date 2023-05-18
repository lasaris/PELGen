using EventLogGenerator.Models;
using EventLogGenerator.Models.Events;
using EventLogGenerator.Models.States;

namespace EventLogGenerator.GenerationLogic;

public static class Collector
{
    public static int LastIndex = -1;
    
    public static List<Dictionary<Actor, List<(ABaseState, DateTime, string?)>>> PassedStates = new();

    public static void CreateCollectorMap()
    {
        var createdDictionary = new Dictionary<Actor, List<(ABaseState, DateTime, string?)>>();
        PassedStates.Add(createdDictionary);
        ++LastIndex;
    }

    public static Dictionary<Actor, List<(ABaseState, DateTime, string?)>> GetPreviousCollection()
    {
        if (!PassedStates.Any())
        {
            throw new ArgumentException("There is no previous collection of logs created");
        }

        return PassedStates[LastIndex - 1];
    }

    public static uint GetLastCollectionMaxId()
    {
        return PassedStates[LastIndex - 1].Keys.Select(actor => actor.Id).Max();
    }

    public static Dictionary<Actor, List<(ABaseState, DateTime, string?)>> GetProcessMap(int index)
    {
        if (index > LastIndex || index < 0)
        {
            throw new ArgumentException("Index out of bounds for CreatedLogs");
        }

        return PassedStates[index];
    }
    
    public static void StateEnteredHandler(object sender, StateEnteredEvent data)
    {
        if (!PassedStates[LastIndex].ContainsKey(data.Actor))
        {
            PassedStates[LastIndex][data.Actor] = new();
        }
        
        PassedStates[LastIndex][data.Actor].Add((data.State, data.TimeStamp, data.Additional));
    }

    // FIXME: This function does 2 things 1) applies rules, thus changing PassedStates 2) logs the traces. Bad design!
    public static void DumpLastProcess()
    {
        var currentProcess = PassedStates[LastIndex];
        var newProcess = new Dictionary<Actor, List<(ABaseState, DateTime, string)>>();
        foreach (var actorStates in currentProcess)
        {
            var evaluatedTrace = RuleEnforcer.GetEvaluatedProcess(actorStates.Value);
            newProcess[actorStates.Key] = evaluatedTrace;
            EventLogger.LogTrace(actorStates.Key, evaluatedTrace);
        }

        PassedStates[LastIndex] = newProcess;
    }
}
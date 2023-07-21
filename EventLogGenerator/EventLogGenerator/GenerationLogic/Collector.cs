using EventLogGenerator.Models;
using EventLogGenerator.Models.Events;
using EventLogGenerator.Models.States;

namespace EventLogGenerator.GenerationLogic;

public static class Collector
{
    public static int LastIndex = -1;
    
    public static List<Dictionary<Actor, List<(ABaseState, DateTime, string?)>>> Processes = new();

    public static void CreateCollectorMap()
    {
        var createdDictionary = new Dictionary<Actor, List<(ABaseState, DateTime, string?)>>();
        Processes.Add(createdDictionary);
        ++LastIndex;
    }

    public static Dictionary<Actor, List<(ABaseState, DateTime, string?)>> GetPreviousCollection()
    {
        if (!Processes.Any())
        {
            throw new ArgumentException("There is no previous collection of logs created");
        }

        return Processes[LastIndex - 1];
    }

    public static uint GetLastCollectionMaxId()
    {
        return Processes[LastIndex - 1].Keys.Select(actor => actor.Id).Max();
    }

    public static void StateEnteredHandler(object sender, StateEnteredEvent data)
    {
        if (!Processes[LastIndex].ContainsKey(data.Actor))
        {
            Processes[LastIndex][data.Actor] = new();
        }
        
        Processes[LastIndex][data.Actor].Add((data.State, data.TimeStamp, data.Additional));
    }

    // FIXME: This function does 2 things 1) applies rules, thus changing PassedStates 2) logs the traces. Bad design!
    public static void DumpLastProcess()
    {
        var currentProcess = Processes[LastIndex];
        var newProcess = new Dictionary<Actor, List<(ABaseState, DateTime, string)>>();
        foreach (var actorStates in currentProcess)
        {
            var evaluatedTrace = RuleEnforcer.GetEvaluatedProcess(actorStates.Value);
            newProcess[actorStates.Key] = evaluatedTrace;
            EventLogger.LogTrace(actorStates.Key, evaluatedTrace);
        }

        Processes[LastIndex] = newProcess;
    }
}
using EventLogGenerationLibrary.InputOutput;
using EventLogGenerationLibrary.Models;
using EventLogGenerationLibrary.Models.Events;
using EventLogGenerationLibrary.Models.States;
using EventLogGenerator.Models;

namespace EventLogGenerationLibrary.GenerationLogic;

/// <summary>
/// Collects processes in ordered List.
/// When ready, you can dump last process, run it through rules and
/// </summary>
internal static class Collector
{
    internal static int LastIndex = -1;
    
    internal static List<Process> Processes = new();

    internal static void CreateCollectorMap()
    {
        var createdDictionary = new Process();
        Processes.Add(createdDictionary);
        ++LastIndex;
    }

    internal  static Process GetPreviousCollection()
    {
        if (!Processes.Any())
        {
            throw new ArgumentException("There is no previous collection of logs created");
        }

        return Processes[LastIndex - 1];
    }

    internal static uint GetLastCollectionMaxId()
    {
        return Processes[LastIndex - 1].Log.Keys.Select(actor => actor.Id).Max();
    }

    internal static void StateEnteredHandler(object sender, StateEnteredArgs data)
    {
        if (!Processes[LastIndex].Log.ContainsKey(data.Actor))
        {
            Processes[LastIndex].Log[data.Actor] = new OrderedTrace();
        }
        
        Processes[LastIndex].Log[data.Actor].Add((data.State, data.TimeStamp, data.Additional));
    }

    // NOTE: This function does 2 things 1) applies rules, thus changing PassedStates 2) logs the traces. Bad design!
    internal static void DumpLastProcess()
    {
        var currentProcess = Processes[LastIndex];
        var newProcess = new Process();
        foreach (var actorStates in currentProcess.Log)
        {
            var evaluatedTrace = RuleEnforcer.GetEvaluatedTrace(actorStates.Value);
            newProcess.Log[actorStates.Key] = evaluatedTrace;
            FileManager.LogTrace(actorStates.Key, evaluatedTrace);
        }

        Processes[LastIndex] = newProcess;
    }
}
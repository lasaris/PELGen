using EventLogGenerator.GenerationLogic;
using EventLogGenerator.Models;

namespace EventLogGenerator;

internal class Program
{

    public static void Main(string[] args)
    {
        StateEvaluator.StateEntered += EventLogger.StateEnteredHandler;
        IsEventsGenerator.GenerateIsLogs(200);
    }
}
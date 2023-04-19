using EventLogGenerator.GenerationLogic;
using EventLogGenerator.Models;
using EventLogGenerator.Services;

namespace EventLogGenerator;

internal class Program
{

    public static void Main(string[] args)
    {
        StateEvaluator.StateEntered += EventLogger.StateEnteredHandler;
        StateEvaluator.StateEntered += SprinkleService.StateEnteredHandler;
        IsEventsGenerator.GenerateIsLogs(200);
    }
}
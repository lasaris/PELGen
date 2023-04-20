using EventLogGenerator.GenerationLogic;
using EventLogGenerator.Models;
using EventLogGenerator.Services;

namespace EventLogGenerator;

internal class Program
{

    public static void Main(string[] args)
    {
        RegisterSubscribers();
        IsEventsGenerator.GenerateIsLogs(200);
    }

    private static void RegisterSubscribers()
    {
        StateEvaluator.StateEntered += EventLogger.StateEnteredHandler;
        StateEvaluator.StateEntered += SprinkleService.StateEnteredHandler;
        SprinkleService.SprinkleAdded += EventLogger.SprinkleAddedHandler;
    }
}
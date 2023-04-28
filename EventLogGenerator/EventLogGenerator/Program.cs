using EventLogGenerator.GenerationLogic;
using EventLogGenerator.Services;

namespace EventLogGenerator;

internal class Program
{

    public static void Main(string[] args)
    {
        RegisterSubscribers();
        IsEventsGenerator.GenerateIsLogs(5);
    }

    private static void RegisterSubscribers()
    {
        StateEvaluator.StateEntered += EventLogger.StateEnteredHandler;
        SprinkleService.SprinkleAdded += EventLogger.SprinkleAddedHandler;
    }
}
using EventLogGenerator.GenerationLogic;
using EventLogGenerator.Services;

namespace EventLogGenerator;

internal class Program
{

    public static void Main(string[] args)
    {
        RegisterSubscribers();
        StudentGenerator.GenerateLogs(30);
        TeacherGenerator.GenerateLogs(3);
    }

    private static void RegisterSubscribers()
    {
        StateEvaluator.StateEntered += EventLogger.StateEnteredHandler;
        SprinkleService.SprinkleAdded += EventLogger.StateEnteredHandler;
        ReactiveStateService.StateEntered += EventLogger.StateEnteredHandler;
        FixedTimeStateService.StateEntered += EventLogger.StateEnteredHandler;
    }
}
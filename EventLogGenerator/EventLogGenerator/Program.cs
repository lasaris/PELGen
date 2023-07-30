using EventLogGenerator.GenerationLogic;
using EventLogGenerator.Generators;
using EventLogGenerator.Services;

namespace EventLogGenerator;

internal class Program
{

    public static void Main(string[] args)
    {
        RegisterSubscribers();
        StudentGenerator.GenerateLogs(200);
        TeacherGenerator.GenerateLogs(3);
    }

    private static void RegisterSubscribers()
    {
        StateEvaluator.StateEntered += Collector.StateEnteredHandler;
        SprinkleService.SprinkleAdded += Collector.StateEnteredHandler;
        ReactiveStateService.StateEntered += Collector.StateEnteredHandler;
        FixedTimeStateService.StateEntered += Collector.StateEnteredHandler;
    }
}
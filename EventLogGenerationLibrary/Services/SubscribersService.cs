using EventLogGenerationLibrary.GenerationLogic;

namespace EventLogGenerationLibrary.Services;

/// <summary>
/// This service makes sure subscribers are registered only once (i.e. when creating multiple EventGenerator objects).
/// </summary>
internal static class SubscribersService
{
    private static bool _areRegistered = false;
    
    internal static void RegisterSubscribers()
    {
        if (_areRegistered)
        {
            return;
        }
        
        StateEvaluator.StateEntered += Collector.StateEnteredHandler;
        SprinkleService.SprinkleAdded += Collector.StateEnteredHandler;
        ReactiveStateService.StateEntered += Collector.StateEnteredHandler;
        FixedTimeStateService.StateEntered += Collector.StateEnteredHandler;
        _areRegistered = true;
    }
}
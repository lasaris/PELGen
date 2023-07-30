using EventLogGenerationLibrary.GenerationLogic;
using EventLogGenerationLibrary.Services;
using EventLogGenerator.Services;

namespace EventLogGenerationLibrary;

public class EventGenerator
{
    
    
    private void ResetServices()
    {
        RuleEnforcer.ResetService();
        ActorService.ResetService();
        FixedTimeStateService.ResetService();
        IdService.ResetService();
        ReactiveStateService.ResetService();
        SprinkleService.ResetService();
    }
}
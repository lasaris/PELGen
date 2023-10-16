using EventLogGenerationLibrary.GenerationLogic;
using EventLogGenerationLibrary.InputOutput;
using EventLogGenerationLibrary.Models;
using EventLogGenerationLibrary.Services;
using EventLogGenerator.Services;

namespace EventLogGenerationLibrary;

public class EventGenerator
{
    private readonly Configuration _configuration;

    public EventGenerator(Configuration configuration)
    {
        _configuration = configuration;
    }

    // FIXME: TODO: This thing could actually return the generated process and that would help us control the dependant processes
    public void RunGeneration()
    {
        Collector.CreateCollectorMap();
        FileManager.SetupNewCsvFile(_configuration.FileHeader, _configuration.FileName);
        StateEvaluator.SetLimits(_configuration.ActivityLimits);
        SubscribersService.RegisterSubscribers();

        List<Actor>? actors = _configuration.Actors;
        if (actors == null)
        {
            if (_configuration.InitialId == null)
            {
                throw new Exception("Invalid process configuration - must contain initial ID or actors");
            }
            
            IdService.SetInitialId((uint)_configuration.InitialId);
            actors = Enumerable.Range(0, _configuration.ActorCount)
                .Select(_ => new Actor(_configuration.ActorType))
                .ToList();
        }

        foreach (var actor in actors)
        {
            var actorFrame = new ActorFrame(actor, _configuration.StartState);
            var filledActorFrame = StateEvaluator.RunProcess(actorFrame);
            SprinkleService.RunSprinkling(filledActorFrame);
            FixedTimeStateService.RunFixedStates(filledActorFrame);
        }  

        ReactiveStateService.RunReactiveStates(Collector.GetPreviousCollection(), actors);
        Collector.DumpLastProcess();
        ResetServices();
    }
    
    private void ResetServices()
    {
        ActorService.ResetService();
        FixedTimeStateService.ResetService();
        ReactiveStateService.ResetService();
        RuleEnforcer.ResetService();
        SprinkleService.ResetService();
        IdService.ResetService();
    }
}
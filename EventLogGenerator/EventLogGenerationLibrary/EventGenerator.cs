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

    public void RunGeneration()
    {
        Collector.CreateCollectorMap();
        FileManager.SetupNewCsvFile(_configuration.FileHeader, _configuration.FileName);
        StateEvaluator.SetLimits(_configuration.ActivityLimits);
        RegisterSubscribers();

        List<Actor>? actors = _configuration.Actors;
        if (actors == null && _configuration.InitialId != null)
        {
            IdService.SetInitialId((uint)_configuration.InitialId);
            actors = Enumerable.Range(0, _configuration.ActorCount)
                .Select(_ => new Actor(_configuration.ActorType))
                .ToList();
        }
        else
        {
            throw new Exception("Invalid actor configuration for process - does not contain initialId nor actors");
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
    
    private static void RegisterSubscribers()
    {
        StateEvaluator.StateEntered += Collector.StateEnteredHandler;
        SprinkleService.SprinkleAdded += Collector.StateEnteredHandler;
        ReactiveStateService.StateEntered += Collector.StateEnteredHandler;
        FixedTimeStateService.StateEntered += Collector.StateEnteredHandler;
    }
}
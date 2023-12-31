﻿using EventLogGenerationLibrary.GenerationLogic;
using EventLogGenerationLibrary.InputOutput;
using EventLogGenerationLibrary.Models;
using EventLogGenerationLibrary.Services;
using EventLogGenerator.Models;
using EventLogGenerator.Services;

namespace EventLogGenerationLibrary;

public class EventGenerator
{
    private readonly Configuration _configuration;

    public EventGenerator(Configuration configuration)
    {
        _configuration = configuration;
    }

    public Process RunGeneration()
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

        // Run reactive states only when previous process is configured
        if (_configuration.ReactToProcess != null)
        {
            // Run reactive states. If we want to react but user has not supplied any strategy, use the first actor available.
            ReactiveStateService.RunReactiveStates(_configuration.ReactToProcess, _configuration.ReactionStrategy ?? new ReactingActorStrategy(actors[0]));
        }
        var newProcess = Collector.DumpLastProcess();
        ResetServices();

        return newProcess;
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
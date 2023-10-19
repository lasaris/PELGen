using EventLogGenerator.Models;

namespace EventLogGenerationLibrary.Models;

public class ReactingActorStrategy
{
    public Actor? SingleReactingActor;

    public Func<Process, Dictionary<Actor, Actor>>? AssignActorsFunction;

    public ReactingActorStrategy(Actor? singleReactingActor = null, Func<Process, Dictionary<Actor, Actor>>? assignActorsFunction = null)
    {
        if (singleReactingActor == null && assignActorsFunction == null)
        {
            throw new ArgumentException("Reacting actor strategy needs at least one parameter filled.");
        }
        
        SingleReactingActor = singleReactingActor;
        AssignActorsFunction = assignActorsFunction;
    }
}
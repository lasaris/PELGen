using EventLogGenerator.Models.States;

namespace EventLogGenerator.Models.Events;

public class SprinkleAddedEvent : EventArgs
{
    public ABaseState Sprinkle;

    public Actor Actor;

    public DateTime TimeStamp;

    public SprinkleAddedEvent(ABaseState state, Actor actor, DateTime timeStamp)
    {
        Sprinkle = state;
        Actor = actor;
        TimeStamp = timeStamp;
    }
}
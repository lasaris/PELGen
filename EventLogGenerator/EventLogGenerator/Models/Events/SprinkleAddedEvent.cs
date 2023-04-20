namespace EventLogGenerator.Models;

public class SprinkleAddedEvent : EventArgs
{
    public SprinkleState Sprinkle;

    public Actor Actor;

    public DateTime TimeStamp;

    public SprinkleAddedEvent(SprinkleState state, Actor actor, DateTime timeStamp)
    {
        Sprinkle = state;
        Actor = actor;
        TimeStamp = timeStamp;
    }
}
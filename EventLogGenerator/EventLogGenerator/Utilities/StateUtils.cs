using EventLogGenerator.Models;

namespace EventLogGenerator.Utilities;

public static class StateUtils
{
    public static ProcessState TransformSprinkleToState(SprinkleState state)
    {
        return new ProcessState(
            state.ActivityType,
            state.Resource,
            new StateRules(), new TimeFrame(new DateTime(0), new DateTime(1)));
    }
}
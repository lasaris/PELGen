using EventLogGenerator.Models;

namespace EventLogGenerator.Utilities;

public static class StateUtils
{
    public static ProcessState TransformSprinkleToState(ABaseState sprinkle)
    {
        return new ProcessState(
            sprinkle.ActivityType,
            sprinkle.Resource,
            new StateRules(), new TimeFrame(new DateTime(0), new DateTime(1))
        );
    }
}
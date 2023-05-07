using EventLogGenerator.Models.Enums;
using EventLogGenerator.Services;

namespace EventLogGenerator.Models.States;

public class PeriodicSprinkleState : ABaseState
{
    // States after which sprinkling is performed
    public HashSet<ProcessState> BeginAfter;

    // States after which sprinkling is stopped
    public HashSet<ProcessState> StopBefore;

    // Time between each sprinkle (beginAfter + Period = first sprinkle time of this state)
    public TimeSpan Period;

    public PeriodicSprinkleState(EActivityType activityType, Resource resource, HashSet<ProcessState> beginAfter, HashSet<ProcessState> stopBefore, TimeSpan period) : base(activityType, resource)
    {
        BeginAfter = beginAfter;
        StopBefore = stopBefore;
        Period = period;

        SprinkleService.LoadPeriodicSprinkle(this);
    }
}
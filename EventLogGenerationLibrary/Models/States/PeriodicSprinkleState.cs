using EventLogGenerationLibrary.Services;
using EventLogGenerator.Services;

namespace EventLogGenerationLibrary.Models.States;

/// <summary>
///  Sprinkle that is periodically added multiple times into the process.
/// The BeginAfter and StopBefore sets define an interval, in which portion of the process should the sprinkle be added.
/// Being sets, this creates an opportunity to add multiple intervals in a single process. The generation starts when
/// a process in BeginAfter is found and ends before StopBefore is discovered.
/// Period defines the time in between each sprinkle addition.
/// </summary>
public class PeriodicSprinkleState : ABaseState
{
    // States after which sprinkling is performed
    public HashSet<ProcessState> BeginAfter;

    // States after which sprinkling is stopped
    public HashSet<ProcessState> StopBefore;

    // Time between each sprinkle (beginAfter + Period = first sprinkle time of this state)
    public TimeSpan Period;

    // NOTE: This could be designed more generally or abstracted
    // Alternative state, chance to occur, max occurrences
    public (ABaseState, float, int)? AlternativeState;

    public PeriodicSprinkleState(string activityType, string resource, HashSet<ProcessState> beginAfter,
        HashSet<ProcessState> stopBefore, TimeSpan period, (ABaseState, float, int)? alternativeState = null) :
        base(activityType, resource)
    {
        BeginAfter = beginAfter;
        StopBefore = stopBefore;
        Period = period;
        AlternativeState = alternativeState;

        SprinkleService.LoadPeriodicSprinkle(this);
    }
}
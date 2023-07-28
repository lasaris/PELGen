using EventLogGenerationLibrary.Services;
using EventLogGenerator.Services;

namespace EventLogGenerationLibrary.Models.States;

/// <summary>
/// The most basic sprinkle state, that simulates the randomness in processes. As the name suggests, its purpose is to
/// randomly add noise to the process, by defining a certain part, where it should randomly occur.
/// 
/// BeginAfter and StopBefore specify the interval in the process where this state can be visited.
/// If some state of the process appears in BeginAfter, that marks the beginning time of the creation, until process
/// contained in StopBefore is found, which marks the end of that timeframe.
/// SkipStart and SkipEnd are optional attributes working in the same principle, that can be used to skip some parts of
/// the process where the sprinkle should not be generated.
/// Passes indicates how many times should the sprinkle be added to the process.
/// </summary>
internal class SprinkleState : ABaseState
{
    // States after which sprinkle can be performed
    internal HashSet<ProcessState> BeginAfter;

    // States after which sprinkle cannot be performed
    internal HashSet<ProcessState> StopBefore;

    // States after which the sprinkle cannot be performed
    internal HashSet<ProcessState>? SkipStart;

    // States that again enable the sprinkle to be performed
    internal HashSet<ProcessState>? SkipEnd;

    // How many passes should this sprinkle be able to perform
    internal int Passes;

    internal SprinkleState(string activityType, string resource, HashSet<ProcessState> beginAfter,
        HashSet<ProcessState> stopBefore, HashSet<ProcessState>? skipStart = null, HashSet<ProcessState>? skipEnd = null,
        Dictionary<ProcessState, float>? afterStateChances = null, int passes = 1) : base(activityType, resource)
    {
        // Cannot sprinkle between same state
        if (beginAfter.Intersect(stopBefore).Any())
        {
            throw new ArgumentException("Cannot create Sprinkle with same beginning and ending state");
        }

        // Passes must be > 0
        if (passes <= 0)
        {
            throw new ArgumentException("Sprinkle must be created with at least 1 pass remaining");
        }

        // Cannot sprinkle after finishing state
        if (beginAfter.Any(state => state.IsFinishing))
        {
            throw new ArgumentException("Sprinkle cannot be added after a finishing state (for now)");
        }

        // Start and end should not overlap
        if (beginAfter.IsSubsetOf(stopBefore) || stopBefore.IsSubsetOf(beginAfter))
        {
            throw new ArgumentException("Start and end of sprinkling overlap with some ProcessState");
        }
        
        // Must have either both skip start & end (or none of them)
        if (skipStart != null && skipEnd == null || skipStart == null && skipEnd != null)
        {
            throw new ArgumentException("Cannot define only skip star or end, they are dependant on each other");
        }

        if (skipStart != null && skipEnd != null)
        {
            // Cover skip interval correctly
            if (skipStart != null && skipStart.Any() && !skipEnd.Any() || !skipStart.Any() && skipEnd.Any())
            {
                throw new ArgumentException("Sprinkle skip interval must have both start and the end");
            }
            
            // Start and end of skip should not overlap
            if (skipStart.IsSubsetOf(skipEnd) || skipEnd.IsSubsetOf(skipStart))
            {
                throw new ArgumentException("Start and end of sprinkling skip should not overlap with some ProcessState");
            }
            
            // Start and end should not contain skipStart or skipEnd states (does not make logical sense to skip at start or end)
            if (skipStart.IsSubsetOf(beginAfter) || beginAfter.IsSubsetOf(skipStart) || skipEnd.IsSubsetOf(stopBefore)
                || stopBefore.IsSubsetOf(skipEnd))
            {
                throw new ArgumentException("Start and end of sprinkling skip overlap with beginning or ending of sprinkle");
            }
        }

        BeginAfter = beginAfter;
        StopBefore = stopBefore;
        SkipStart = skipStart;
        SkipEnd = skipEnd;
        Passes = passes;

        SprinkleService.LoadSprinkleState(this);
    }
}
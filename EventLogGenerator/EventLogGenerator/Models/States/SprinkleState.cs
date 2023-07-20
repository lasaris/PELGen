using EventLogGenerator.Models.Enums;
using EventLogGenerator.Models.States;
using EventLogGenerator.Services;

namespace EventLogGenerator.Models;

public class SprinkleState : ABaseState
{
    // States after which sprinkle can be performed
    public HashSet<ProcessState> BeginAfter;

    // States after which sprinkle cannot be performed
    public HashSet<ProcessState> StopBefore;

    // States after which the sprinkle cannot be performed
    public HashSet<ProcessState>? SkipStart;

    // States that again enable the sprinkle to be performed
    public HashSet<ProcessState>? SkipEnd;

    // How many passes should this sprinkle be able to perform
    public int Passes;

    public SprinkleState(EActivityType activityType, Resource resource, HashSet<ProcessState> beginAfter,
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
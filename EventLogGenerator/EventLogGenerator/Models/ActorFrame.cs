namespace EventLogGenerator.Models;

/// <summary>
/// This frame moves along states with the actor. Acts as a
/// </summary>
public class ActorFrame
{
    public Actor Actor;

    public ProcessState? CurrentCase;
    
    // TODO: Next() function to jump to next state after evaluation from StateEvaluator()
}
namespace EventLogGenerator.Exceptions;

public class InvalidSprinklerState : Exception
{
    public InvalidSprinklerState(string? message) : base(message)
    {
    }
}
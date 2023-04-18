namespace EventLogGenerator.Exceptions;

public class InvalidProcessStateException : Exception
{
    public InvalidProcessStateException(string? message) : base(message)
    {
    }
}
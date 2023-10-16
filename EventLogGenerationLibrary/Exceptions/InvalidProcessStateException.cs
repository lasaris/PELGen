namespace EventLogGenerator.Exceptions;

internal class InvalidProcessStateException : Exception
{
    internal InvalidProcessStateException(string? message) : base(message)
    {
    }
}
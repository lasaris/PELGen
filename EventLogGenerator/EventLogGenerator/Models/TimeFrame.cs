namespace EventLogGenerator.Models;

public class TimeFrame
{
    public DateTime Start;

    public DateTime End;

    public TimeFrame(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }
}
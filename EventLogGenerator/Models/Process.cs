using System.Diagnostics;

namespace EventLogGenerator.Models;

public class Process
{
    public Dictionary<Actor, Trace> Log;

    public Process(Dictionary<Actor, Trace> log)
    {
        Log = log;
    }
}
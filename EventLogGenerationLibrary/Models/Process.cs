using System.Net.Sockets;
using EventLogGenerationLibrary.Models;

namespace EventLogGenerator.Models;

/// <summary>
/// Represents a whole process with actors and their traces.
/// </summary>
public class Process
{
    // Collection to keep the process
    internal Dictionary<Actor, OrderedTrace> Log;

    public Process()
    {
        Log = new Dictionary<Actor, OrderedTrace>();
    }

    internal Process(Dictionary<Actor, OrderedTrace> log)
    {
        Log = log;
    }
}
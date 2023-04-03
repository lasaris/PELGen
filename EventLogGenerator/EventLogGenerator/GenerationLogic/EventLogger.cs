using System.Text;
using EventLogGenerator.InputOutput;
using EventLogGenerator.Models;
using EventLogGenerator.Services;

namespace EventLogGenerator.GenerationLogic;

/// <summary>
/// Listens for events and then logs them into specified log file
/// </summary>
public static class EventLogger
{
    // Dictionary of processes and their enter timestamps (this model is somehow open for future parallel computing)
    private static Dictionary<ProcessState, DateTime> _lastEntered;

    public static void StateEnteredHandler(object sender, StateEnteredEvent data)
    {
        if (!_lastEntered.TryAdd(data.State, data.TimeStamp))
        {
            throw new ArgumentException("Trying to register event start, that was already entered and not properly finished");
        }
    }

    public static void StateExitedHandler(object sender, StateExitedEvent data)
    {
        // Get timestamp of state enter
        if (!_lastEntered.TryGetValue(data.State, out var stateEnteredTime))
        {
            throw new ArgumentException("Trying to exit a state that was not previuously registered as entered");
        }
        
        // Prepare string
        var sb = new StringBuilder();
        var eventId = IdService.GetNewEventId();
        sb.Append(eventId + ",");
        sb.Append(data.State.ActivityType + ",");
        sb.Append(data.Actor.Id + ",");
        sb.Append(data.Actor.Type + ",");
        sb.Append(data.State.Resource + ",");
        sb.Append(stateEnteredTime + ",");
        sb.Append(data.TimeStamp);
        
        // Write string to CSV file
        FileManager.AppendLineToCsv(sb.ToString());
        
        // Clean dictionary
        _lastEntered = new Dictionary<ProcessState, DateTime>();
    }
}
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
    public static void StateEnteredHandler(object sender, StateEnteredEvent data)
    {
        // Prepare string
        var sb = new StringBuilder();
        sb.Append(data.Actor.Id + ",");
        sb.Append(data.Actor.Type + ",");
        sb.Append(data.State.ActivityType + ",");
        sb.Append(data.State.Resource + ",");
        sb.Append(data.TimeStamp + ",");
        
        // Write string to CSV file
        FileManager.AppendLineToCsv(sb.ToString());
    }
}
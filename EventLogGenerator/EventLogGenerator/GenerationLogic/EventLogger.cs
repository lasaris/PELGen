using System.Text;
using EventLogGenerator.InputOutput;
using EventLogGenerator.Models;
using EventLogGenerator.Utilities;

namespace EventLogGenerator.GenerationLogic;

/// <summary>
/// Listens for events and then logs them into specified log file
/// </summary>
public static class EventLogger
{
    // FIXME: This function should not be needed since StateEnteredEvent already takes generalized ABaseState
    public static void SprinkleAddedHandler(object sender, SprinkleAddedEvent data)
    {
        var processStateFromSprinkle = StateUtils.TransformSprinkleToState(data.Sprinkle);
        var newProcessStateEvent = new StateEnteredEvent(processStateFromSprinkle, data.Actor, data.TimeStamp);

        StateEnteredHandler(sender, newProcessStateEvent);
    }

    public static void StateEnteredHandler(object sender, StateEnteredEvent data)
    {
        // Prepare string
        var sb = new StringBuilder();
        sb.Append(data.Actor.Id + ",");
        sb.Append(data.Actor.Type + ",");
        sb.Append(data.State.ActivityType + ",");
        sb.Append(data.State.Resource.Name + ",");
        sb.Append(data.TimeStamp);

        // Note in logs collector
        Collector.AddLog(data.Actor.Id, data.State, data.TimeStamp);
        
        // Write string to CSV file
        FileManager.AppendLineToCsv(sb.ToString());
    }
}
using System.Text;
using EventLogGenerator.InputOutput;
using EventLogGenerator.Models.Enums;
using EventLogGenerator.Models.Events;
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
        if (data.Additional != null)
        {
            sb.Append("," + data.Additional);
        }

        // FIXME: Make more general or design somehow else. This hardcoded shit is fucked up man.
        var StudentFiles = new List<string>()
        {
            "/ode/homework-1.zip", "/ode/homework-2.zip", "/ode/homework-3.zip"
        };

        var TeacherFIles = new List<string>()
        {
            "/um/slides-week01.pdf",
            "/um/slides-week02.pdf",
            "/um/slides-week03.pdf",
            "/um/slides-week04.pdf",
            "/um/slides-week05.pdf",
            "/um/slides-week06.pdf",
        };
        
        if (StudentFiles.Contains(data.State.Resource.Name))
        {
            if (data.Actor.Type == EActorType.Student)
            {
                sb.Append($",{data.Actor.Id}");
            } else if (data.Additional != null)
            {
                sb.Append($",{data.Additional}");
            }
        } else if (TeacherFIles.Contains(data.State.Resource.Name))
        {
            if (data.Additional == null && data.Actor.Type == EActorType.Teacher)
            {
                sb.Append(",,1");
            }
            else
            {
                sb.Append(",1");
            }
        }

        // Note in logs collector
        Collector.AddLog(data.Actor.Id, data.State, data.TimeStamp);

        // Write string to CSV file
        FileManager.AppendLineToCsv(sb.ToString());
    }
}
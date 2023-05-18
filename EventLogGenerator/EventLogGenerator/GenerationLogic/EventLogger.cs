using System.Text;
using EventLogGenerator.InputOutput;
using EventLogGenerator.Models;
using EventLogGenerator.Models.Enums;
using EventLogGenerator.Models.Events;
using EventLogGenerator.Models.States;
using EventLogGenerator.Utilities;

namespace EventLogGenerator.GenerationLogic;

/// <summary>
/// Listens for events and then logs them into specified log file
/// </summary>
public static class EventLogger
{
    public static void LogTrace(Actor actor, List<(ABaseState, DateTime, string?)> evaluatedTrace)
    {
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
        
        var sbLogs = new StringBuilder();
        foreach (var stateLog in evaluatedTrace)
        {
            var sb = new StringBuilder();
            
            sb.Append(actor.Id + ",");
            sb.Append(actor.Type + ",");
            sb.Append(stateLog.Item1.ActivityType + ",");
            sb.Append(stateLog.Item1.Resource.Name + ",");
            sb.Append(stateLog.Item2);
            if (stateLog.Item3 != null)
            {
                sb.Append("," + stateLog.Item3);
            }
            
            if (StudentFiles.Contains(stateLog.Item1.Resource.Name))
            {
                if (actor.Type == EActorType.Student)
                {
                    sb.Append($",{actor.Id}");
                } else if (stateLog.Item3 != null)
                {
                    sb.Append($",{stateLog.Item3}");
                }
            } else if (TeacherFIles.Contains(stateLog.Item1.Resource.Name))
            {
                if (stateLog.Item3 == null && actor.Type == EActorType.Teacher)
                {
                    sb.Append(",,514184");
                }
                else
                {
                    sb.Append(",514184");
                }
            }
            
            // FIXME: Is this optimal?
            var lineColumnCount = sb.ToString().Split(',').Count() + 1;
            while (FileManager.ColumnCount > lineColumnCount)
            {
                sb.Append(",");
                ++lineColumnCount;
            }
            
            sb.Append('\n');
            sbLogs.Append(sb.ToString());
        }
        // Write into CSV file
        FileManager.AddLogs(sbLogs.ToString());
    }
}
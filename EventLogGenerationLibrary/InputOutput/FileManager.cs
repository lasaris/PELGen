using System.Text;
using EventLogGenerationLibrary.Models;

namespace EventLogGenerationLibrary.InputOutput;

/// <summary>
/// Handles all file operations and logging.
/// </summary>
internal static class FileManager
{
    internal static string OutputFileName = "output.csv";

    internal static string OutputFolderName = "generated";

    internal static int ColumnCount = 0;
    
    private static void AppendLine(string line)
    {
        string outPath = Path.Combine(OutputFolderName, OutputFileName);

        using (StreamWriter writer = new StreamWriter(outPath, true))
        {
            writer.WriteLine(line);
        }
    }

    /// <summary>
    /// Creates new CSV file and appends given header to it
    /// CSV file must be prepared before trying to append lines to it via "AppendLineToCsv"
    /// </summary>
    /// <param name="headerLine">header for CSV file</param>
    /// <param name="filename">name of newly created file</param>
    internal static void SetupNewCsvFile(string headerLine, string? filename = null)
    {
        OutputFileName = filename ?? OutputFileName;
        Directory.CreateDirectory(OutputFolderName);
        if (File.Exists(Path.Combine(OutputFolderName, OutputFileName)))
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(OutputFolderName, OutputFileName)))
            {
                sw.Write("");
            }
        }

        ColumnCount = headerLine.Split(',').Count() + 1;
        AppendLine(headerLine);
    }
    
    internal static void AddLogs(string logs)
    {
        string outPath = Path.Combine(OutputFolderName, OutputFileName);

        using (StreamWriter writer = new StreamWriter(outPath, true))
        {
            writer.Write(logs);
        }
    }

    internal static void LogTrace(Actor actor, OrderedTrace evaluatedTrace)
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
        foreach (var stateLog in evaluatedTrace.Trace)
        {
            var sb = new StringBuilder();

            sb.Append(actor.Id + ",");
            sb.Append(actor.Type + ",");
            sb.Append(stateLog.State.ActivityType + ",");
            sb.Append(stateLog.State.Resource + ",");
            sb.Append(stateLog.Time);
            if (stateLog.Additional != null)
            {
                sb.Append("," + stateLog.Additional);
            }

            if (StudentFiles.Contains(stateLog.State.Resource) && (actor.Type == "Student"))
            {
                sb.Append($",{actor.Id}");
            }
            else if (TeacherFIles.Contains(stateLog.State.Resource))
            {
                if (stateLog.Additional == null && actor.Type == "Teacher")
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
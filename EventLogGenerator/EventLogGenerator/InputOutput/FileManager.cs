namespace EventLogGenerator.InputOutput;

public static class FileManager
{
    public static string OutputFileName = "output.csv";

    public static string OutputFolderName = "generated";

    public static int ColumnCount = 0;
    
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
    public static void SetupNewCsvFile(string headerLine, string? filename = null)
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
    
    public static void AddLogs(string logs)
    {
        string outPath = Path.Combine(OutputFolderName, OutputFileName);

        using (StreamWriter writer = new StreamWriter(outPath, true))
        {
            writer.Write(logs);
        }
    }
}
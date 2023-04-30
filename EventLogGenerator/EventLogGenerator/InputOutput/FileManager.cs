namespace EventLogGenerator.InputOutput;

public static class FileManager
{
    public static string OutputFileName = "output.csv";

    public static string OutputFolderName = "generated";
    
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

        AppendLine(headerLine);
    }

    public static void SetOutputCsvPath(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new ArgumentException("File must exist!");
        }
        
        OutputFileName = filePath;
    }

    /// <summary>
    /// Safely tries to append line to already created CSV file.
    /// </summary>
    /// <param name="line">valid CSV line of data for given file</param>
    /// <param name="filename">name of file that line should be appended to</param>
    /// <exception cref="ArgumentException">Thrown on invalid filename, file format or line data format</exception>
    public static void AppendLineToCsv(string line)
    {
        string outPath = Path.Combine(OutputFolderName, OutputFileName);

        if (!File.Exists(outPath))
        {
            throw new ArgumentException("Provided file does not exist");
        }

        // FIXME: Could this be optimized so we don't open file on every write?
        using (var reader = new StreamReader(outPath))
        {
            var headerLine = reader.ReadLine();
            if (headerLine == null)
            {
                throw new ArgumentException("Given CSV file has no header");
            }

            var columnCount = headerLine.Split(',').Length;
            var lineColumnCount = line.Split(',').Length;

            if (columnCount != lineColumnCount)
            {
                throw new ArgumentException(
                    $"Given CSV file has {columnCount} columns, but in file there were {lineColumnCount} given");
            }
        }

        AppendLine(line);
    }
}
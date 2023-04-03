namespace EventLogGenerator.InputOutput;

public static class FileManager
{
    public static string OutputFileName = "output.csv";

    public static string OutputFolderName = "generated";

    /// <summary>
    /// Creates new CSV file and appends given header to it
    /// CSV file must be prepared before trying to append lines to it via "AppendLineToCsv"
    /// </summary>
    /// <param name="headerLine">header for CSV file</param>
    /// <param name="filename">name of newly created file</param>
    public static void SetupNewCsvFile(string headerLine, string? filename = null)
    {
        filename ??= OutputFileName;
        Directory.CreateDirectory(OutputFolderName);
        AppendLine(headerLine);
    }

    /// <summary>
    /// Safely tries to append line to already created CSV file.
    /// </summary>
    /// <param name="line">valid CSV line of data for given file</param>
    /// <param name="filename">name of file that line should be appended to</param>
    /// <exception cref="ArgumentException">Thrown on invalid filename, file format or line data format</exception>
    public static void AppendLineToCsv(string line, string? filename = null)
    {
        filename ??= OutputFileName;
        string outPath = Path.Combine(OutputFolderName, filename);

        if (!File.Exists(outPath))
        {
            throw new ArgumentException("Provided file does not exist");
        }

        using var reader = new StreamReader(outPath);
        var headerLine = reader.ReadLine();
        if (headerLine == null)
        {
            throw new ArgumentException("Given CSV file has no header");
        }
        
        var columnCount = headerLine.Split(',').Length;
        var lineColumnCount = line.Split(',').Length;

        if (columnCount != lineColumnCount)
        {
            throw new ArgumentException($"Given CSV file has {columnCount} columns, but in file there were {lineColumnCount} given");
        }
        
        AppendLine(line, filename);
    }
    
    private static void AppendLine(string line, string? filename = null)
    {
        filename ??= OutputFileName;
        string outPath = Path.Combine(OutputFolderName, filename);
        if (!File.Exists(outPath))
        {
            File.Create(outPath);
        }
        
        using StreamWriter writer = new StreamWriter(outPath, true);
        writer.WriteLine(line);
    }
}
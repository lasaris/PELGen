namespace EventLogGenerator.InputOutput;

public static class FileManager
{
    public static string OutputFileName = "output.csv";

    public static string OutputFolderName = "generated";
    
    public static void AppendLine(string line, string? filename = null)
    {
        filename ??= OutputFileName;
        
        string outputPath = Path.Combine(OutputFolderName, filename);
        Directory.CreateDirectory(OutputFolderName);

        using StreamWriter writer = new StreamWriter(outputPath, true);
        writer.WriteLine(line);
    }
}
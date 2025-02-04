using System;
using System.IO;


public static class Logger
{
    private static readonly string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs");

    public static void Log(Save save, int fileSize, int transferTime)
    {
        string logFile = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.json");
        Directory.CreateDirectory(logDirectory);

        string logEntry = $"{{\"timestamp\":\"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\",\"saveName\":\"{save.name}\",\"source\":\"{save.sourceRepository}\",\"destination\":\"{save.targetRepository}\",\"size\":{fileSize},\"timeMs\":{transferTime}}}";
        Console.WriteLine(logEntry);
        File.AppendAllText(logFile, logEntry + Environment.NewLine);
    }
}


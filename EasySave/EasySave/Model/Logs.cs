using System;
using System.IO;


public static class Logger
{
    private static readonly string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs");

    public static void Log(string saveName, string sourcePath, string destPath, long fileSize, long transferTime)
    {
        string logFile = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.json");
        Directory.CreateDirectory(logDirectory);

        string logEntry = $"{{\"timestamp\":\"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\",\"saveName\":\"{saveName}\",\"source\":\"{sourcePath}\",\"destination\":\"{destPath}\",\"size\":{fileSize},\"timeMs\":{transferTime}}}";

        File.AppendAllText(logFile, logEntry + Environment.NewLine);
    }
}


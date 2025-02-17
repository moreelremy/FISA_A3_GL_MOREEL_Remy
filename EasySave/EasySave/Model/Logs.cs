using System.Text.Json;
using EasySaveLogger;

public static class Logs
{
    /// <summary>
    /// Represents a log entry for a backup operation.
    /// </summary>
    public class LogEntry
    {
        public string? timestamp { get; set; }
        public string? saveName { get; set; }
        public string? source { get; set; }
        public string? target { get; set; }
        public long? size { get; set; }
        public int? transferTimeMs { get; set; }
    }

    /// <summary>
    /// use to enter daily logs
    /// </summary>
    /// <param name="save">The save object.</param>
    /// <param name="fileSize">The size of the transferred file in bytes.</param>
    /// <param name="transferTime">The time taken for the file transfer in milliseconds.</param>
    public static void GeneralLog(Save save, long fileSize, int transferTime)
    {
        Logger.Log(
            new Dictionary<string, object>
            {
                { "timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                { "saveName", save.name },
                { "source", Logs.ConvertToUnc(save.sourceDirectory) },
                { "target", Logs.ConvertToUnc(save.targetDirectory) },
                { "size", fileSize },
                { "transferTimeMs", transferTime }
            },
            $"Logs/{DateTime.Now:yyyy-MM-dd}.xml");
    }

    /// <summary>
    /// Reads log entries from a specified log file.
    /// <param name="filePath">The path to the log file to be read.</param>
    /// <returns>
    /// A list of log entries in reverse order (most recent first).
    /// If an error occurs, the list contains an error message.
    /// </returns>
    public static List<LogEntry> ReadGeneralLog(string filePath)
    {
        try
        {
            List<LogEntry> logs = new List<LogEntry>();
            using JsonDocument jsonContent = JsonDocument.Parse(File.ReadAllText(filePath));
            foreach (JsonElement jsonElement in jsonContent.RootElement.EnumerateArray())
            {
                logs.Add(
                   new LogEntry
                   {
                       timestamp = jsonElement.GetProperty("timestamp").GetString(),
                       saveName = jsonElement.GetProperty("saveName").GetString(),
                       source = jsonElement.GetProperty("source").GetString(),
                       target = jsonElement.GetProperty("target").GetString(),
                       size = jsonElement.GetProperty("size").GetInt64(),
                       transferTimeMs = jsonElement.GetProperty("transferTimeMs").GetInt32()
                   }
               );
            }
            return logs.AsEnumerable().Reverse().ToList();
        }

        catch (Exception ex)
        {
            return new List<LogEntry> { new LogEntry { timestamp = "Erreur", saveName = ex.Message } };
        }
    }

    public static string ConvertToUnc(string localPath)
    {
        if (localPath == "")
            return "";

        string fullPath = Path.GetFullPath(localPath);

        if (!Path.IsPathRooted(fullPath))
            throw new ArgumentException("Le chemin fourni n'est pas absolu.");

        string driveLetter = Path.GetPathRoot(fullPath)?.TrimEnd('\\');
        if (driveLetter == null || driveLetter.Length < 2)
            throw new ArgumentException("Impossible d'extraire la lettre du disque.");

        string uncPath = fullPath.Replace(driveLetter, $"\\\\localhost\\{driveLetter.TrimEnd(':')}$");

        return uncPath;
    }

    /// <summary>
    /// Logs real-time information about a file transfer process.
    /// </summary>
    /// <param name="save">The save object.</param>
    /// <param name="fileSize">The size of the currently transferred file in bytes.</param>
    /// <param name="transferTime">The time taken for the current file transfer in milliseconds.</param>
    /// <param name="state">The current state of the transfer process.(END or ACTIVE)</param>
    /// <param name="totalFileSizetoCopy">The total size of files to be copied in bytes.</param>
    /// <param name="totalFileSize">The total size of all files involved in the process in bytes.</param>
    /// <param name="nbFilesLeftToDo">The number of remaining files to be transferred.</param>
    /// <param name="Progression">The percentage of completion of the file transfer.</param>
    public static void RealTimeLog(
        string saveName,
        string sourcePath,
        string targetPath,
        long fileSize,
        string state,
        int totalFilesToCopy,
        long totalFileSize,
        int nbFilesLeftToDo,
        long filesSizeLeftToDo,
        int Progression
        )
    {
        Logger.Log(
            new Dictionary<string, object>
            {
                { "saveName", saveName },
                { "sourceFilePath", Logs.ConvertToUnc(sourcePath) },
                { "targetFilePath", Logs.ConvertToUnc(targetPath) },
                { "fileSize", fileSize },
                { "state", state },
                { "totalFilesToCopy", totalFilesToCopy },
                { "totalFilesSize", totalFileSize },
                { "nbFilesLeftToDo", nbFilesLeftToDo },
                { "filesSizeLeftToDo", filesSizeLeftToDo },
                { "timestamp", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") },
                { "Progression", Progression }
            },
            $"RealTime/state.json");
    }
}
using System;
using System.IO;
using System.Text;
using EasySaveLogger;

public static class Logs
{
    /// <summary>
    /// use to enter daily logs
    /// </summary>
    /// <param name="save">The save object.</param>
    /// <param name="fileSize">The size of the transferred file in bytes.</param>
    /// <param name="transferTime">The time taken for the file transfer in milliseconds.</param>
    public static void GeneralLog(Save save, long fileSize, int transferTime)
    {

        string logEntry = $"{{\"timestamp\":\"{DateTime.Now:dd-MM-yyyy HH:mm:ss}\",\"saveName\":\"{save.name}\",\"source\":\"{save.sourceDirectory}\",\"target\":\"{save.targetDirectory}\",\"size\":{fileSize},\"transferTimeMs\":{transferTime}}}";
        Logger.Log(logEntry, $"Logs/{DateTime.Now:dd-MM-yyyy}.json");

    }

    /// <summary>
    /// Reads log entries from a specified log file.
    /// <param name="filePath">The path to the log file to be read.</param>
    /// <returns>
    /// A list of log entries in reverse order (most recent first).
    /// If an error occurs, the list contains an error message.
    /// </returns>
    public static List<string> ReadGeneralLog(string filePath)
    {

        StreamReader? reader = null;
        List<string> logLines = new List<string>();

        try
        {
            reader = new StreamReader(filePath);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                logLines.Add(line);
            }
            return logLines.AsEnumerable().Reverse().ToList();
        }
        catch (Exception ex)
        {
            return new List<string> { "Erreur lors de la lecture du fichier : " + ex.Message };
        }
        finally
        {
            if (reader != null)
            {
                reader.Close(); // Close the file to free resources
            }
        }
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
        string logEntry = $"{{" +
            $"\"Name\":\"{saveName}\"," +
            $"\"SourceFilePath\":\"{sourcePath}\"," +
            $"\"TargetFilePath\":\"{targetPath}\"," +
            $"\"FileSize\":{fileSize}," +
            $"\"State\":\"{state}\"," +
            $"\"TotalFilesToCopy\":{totalFilesToCopy}," +
            $"\"TotalFilesSize\":{totalFileSize}," +
            $"\"NbFilesLeftToDo\":{nbFilesLeftToDo}," +
            $"\"FilesSizeLeftToDo\":{filesSizeLeftToDo}," +
            $"\"timestamp\":\"{DateTime.Now:dd-MM-yyyy HH:mm:ss}\"," +
            $"\"Progression\":{Progression}" +
            "}";
 
        Logger.Log(logEntry, $"RealTime/state.json");

    }

}
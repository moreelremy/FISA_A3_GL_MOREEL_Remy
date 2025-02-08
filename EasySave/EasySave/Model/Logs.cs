using System;
using System.IO;
using System.Text;
using EasySaveLogger;

public static class Logs
{
    public static void GeneralLog(Save save, int fileSize, int transferTime, string? pathFile = null)
    {

        string logEntry = $"{{\"timestamp\":\"{DateTime.Now:dd-MM-yyyy HH:mm:ss}\",\"saveName\":\"{save.name}\",\"source\":\"{save.sourceDirectory}\",\"destination\":\"{save.targetDirectory}\",\"size\":{fileSize},\"timeMs\":{transferTime}}}";

        Logger.Log(logEntry);

    }
    /*
    public static void ReadGeneralLog(string wantedDate)
    {
        string filePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs", wantedDate));

        using (StreamReader reader = new StreamReader(filePath))
        {
            string firstLine = reader.ReadLine();
            Console.WriteLine("Première ligne du fichier JSON : " + firstLine);
        }
    }
    */
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

}
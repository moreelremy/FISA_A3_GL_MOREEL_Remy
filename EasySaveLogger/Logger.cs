
using System.Text.Json;


namespace EasySaveLogger
{
    public static class Logger
    {
        public static void Log(string jsonEntry, string filePath)
        {
            string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs/", filePath);

            Directory.CreateDirectory(Path.GetDirectoryName(pathFile));

            List<Dictionary<string, object>> logs;

            try
            {
                //read the entire file 
                string existingJson = File.ReadAllText(pathFile);
                // Deserialize JSON into a list of dictionaries or initialize an empty list if null
                logs = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(existingJson) ?? new List<Dictionary<string, object>>();

            }
            catch
            {
                logs = new List<Dictionary<string, object>>();
            }

            // Convert JSON entry in Dictionary
            var newEntry = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonEntry);
            if (newEntry != null)
            {
                logs.Add(newEntry);
            }

            // Create options for indented JSON
            var options = new JsonSerializerOptions { WriteIndented = true };

            // Serialize 'logs' to JSON and write it to a file with a new line at the end
            File.WriteAllText(pathFile, JsonSerializer.Serialize(logs, options) + Environment.NewLine);


        }
    }
}
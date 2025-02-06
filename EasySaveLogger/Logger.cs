using System;
using System.IO;

namespace EasySaveLogger
{
    public static class Logger
    {
        public static void Log(string jsonEntry, string? pathFile = null)
        {
            // add a default value if pathFile is null
            pathFile ??= Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs", $"{DateTime.Now:yyyy-MM-dd}.json");




            Directory.CreateDirectory(Path.GetDirectoryName(pathFile));

            File.AppendAllText(pathFile, jsonEntry + Environment.NewLine);

        }
    }
}
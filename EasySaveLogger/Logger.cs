using System;
using System.IO;

namespace EasySaveLogger
{
    public static class Logger
    {
        public static void Log(string jsonEntry, string? pathFile = null)
        {
            pathFile = (pathFile == null)
                ? Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs"), $"{DateTime.Now:yyyy-MM-dd}.json")
                : pathFile;



            Directory.CreateDirectory(Path.GetDirectoryName(pathFile));

            File.AppendAllText(pathFile, jsonEntry + Environment.NewLine);

        }
    }
}
using System;
using System.IO;

namespace EasySaveLogger
{
    public static class Logger
    {
        public static void Log(string jsonEntry)
        {
            string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs/Logs", $"{DateTime.Now:dd-MM-yyyy}.json");

            Directory.CreateDirectory(Path.GetDirectoryName(pathFile));

            File.AppendAllText(pathFile, jsonEntry + Environment.NewLine);

        }

        public static void RealTime(string jsonEntry)
        {
            string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs/RealTime", "state.json");

            Directory.CreateDirectory(Path.GetDirectoryName(pathFile));

            File.AppendAllText(pathFile, jsonEntry + Environment.NewLine);
        }

    }
}
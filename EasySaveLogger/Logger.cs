using System;
using System.IO;

namespace EasySaveLogger
{
    public static class Logger
    {
        public static void Log(string jsonEntry, string filePath)
        {
            //string test = $"Logs/{DateTime.Now:dd-MM-yyyy}.json";
            string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs/", filePath);

            Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
            File.AppendAllText(pathFile, jsonEntry + Environment.NewLine);
        }
    }
}
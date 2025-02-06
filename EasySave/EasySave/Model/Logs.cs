using System;
using System.IO;
//using EasySaveLogger;

public static class Log
{
    public static void GeneralLog(Save save, int fileSize, int transferTime)
    {

        string logEntry = $"{{\"timestamp\":\"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\",\"saveName\":\"{save.name}\",\"source\":\"{save.sourceRepository}\",\"destination\":\"{save.targetRepository}\",\"size\":{fileSize},\"timeMs\":{transferTime}}}";

        //EasySaveLogger.Logger.Log(logEntry);

    }
}

//mettre en parametre le pathFile et le string json
using System;

class Program
{
    static void Main(string[] args)
    {
        bool leave = false;
        while (!leave)
        {
            string response = View.ShowMenu();

            switch (response)
            {
                case "1":
                    Console.WriteLine(Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs"));
                    Logger.Log("Backup1", @"C:\Source\File.txt", @"D:\Backup\File.txt", 1024, 500);
                    Console.WriteLine($"Répertoire actuel : {Directory.GetCurrentDirectory()}");
                    break;

                case "2":
                    break;

                case "3":
                    break;

                case "4":
                    leave = true;
                    break;

            }
        }
    }
}
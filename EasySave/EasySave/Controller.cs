using System;

class Controler
{
    static void Main(string[] args)
    {

        bool leave = false;

        //Exemple de création d'une save
        FullSave fullSave = new FullSave();
        //Save Backup1 = null;

        while (!leave)
        {
            string response = View.ShowMenu();  
            
            switch (response)
            {
                case "1":
                    /*
                    Backup1 = new Save
                    {
                        name = "Backup1",
                        sourceDirectory = @"C:\TestSourceDir",
                        targetDirectory = @"C:\TestTargetDir",
                        saveStrategy = fullSave
                    };
                    */
                    Console.WriteLine(Language.GetString("BackupCreated"));
                    break;

                case "2":
                    Console.WriteLine(Language.GetString("BackupStarted"));
                    /*
                    if (Backup1 == null)
                    {
                        Console.WriteLine(Language.GetString("NoBackup"));
                        break;
                    } else
                    {
                        Backup1.saveStrategy.Save(Backup1);
                    }
                    */
                    break;

                case "3":
                    Console.WriteLine(Language.GetString("ViewLogs"));
                    Console.WriteLine(Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs"));
                    Logger.Log("Backup1", @"C:\Source\File.txt", @"D:\Backup\File.txt", 1024, 500);
                    Console.WriteLine($"Répertoire actuel : {Directory.GetCurrentDirectory()}");
                    break;

                case "4":
                    // Change the language with the model
                    Language.SetLanguage(View.GetLanguageChoice());
                    break;

                case "5":
                    leave = true;
                    break;

                default:
                    Console.WriteLine(Language.GetString("InvalidChoice"));
                    break;
            }
            Console.Clear();
        }
    }
}

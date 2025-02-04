using System;

class Controler
{
    static void Main(string[] args)
    {

        bool leave = false;
        while (!leave)
        {
            string response = View.ShowMenu();

            //Exemple de création d'une save
            FullSave fullSave = new FullSave();
            DifferentialSave differentialSave = new DifferentialSave();
            Save save = new Save
            {
                Name = "Backup1",
                SourceRepository = @"C:\Source\File.txt",
                TargetRepository = @"D:\Backup\File.txt",
                SaveType = fullSave,
                DateSauvegarde = DateTime.Now
            };

            switch (response)
            {
                case "1":
                    Console.WriteLine(Language.GetString("BackupCreated"));
                    break;

                case "2":
                    Console.WriteLine(Language.GetString("BackupStarted"));
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

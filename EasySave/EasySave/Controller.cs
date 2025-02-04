using System;

class Controller
{
    static void Main(string[] args)
    {
        SaveRepository saveRepository = new SaveRepository();

        bool leave = false;
        while (!leave)
        {
            string response = View.ShowMenu();

            //Exemple de création d'une save
            /*FullSave fullSave = new FullSave();
            DifferentialSave differentialSave = new DifferentialSave();
            Save save = new Save
            {
                name = "Backup1",
                sourceRepository = @"C:\Source\File.txt",
                targetRepository = @"D:\Backup\File.txt",
                saveType = fullSave,
                dateSauvegarde = DateTime.Now
            };
            */

            switch (response)
            {
                case "1":
                    Console.WriteLine(Language.GetString("SaveAdded"));
                    saveRepository.AjouterSave();


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
        }
    }
}

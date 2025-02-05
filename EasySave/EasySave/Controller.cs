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
                    // Use view to create a new save
                    Save newSave = View.CreateBackupView();

                    // Add save to the model
                    Save addedSave = saveRepository.AjouterSave(newSave);

                    // View the added save
                    View.SaveAddedMessageView(addedSave);
                    

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
                    // View all saves
                    List<Save> saves = saveRepository.ObtenirToutesLesSaves();
                    View.AfficherSavesView(saves);

                    Console.WriteLine(Language.GetString("PressAnyKey"));
                    Console.ReadLine();
                    break;

                case "6":
                    leave = true;
                    break;

                


                default:
                    Console.WriteLine(Language.GetString("InvalidChoice"));
                    break;
            }
        }
    }
}

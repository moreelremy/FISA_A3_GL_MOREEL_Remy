using System;

class Controller
{
    static void Main(string[] args)
    {
        SaveRepository saveRepository = new SaveRepository();

        bool leave = false;

        //Exemple de création d'une save
        FullSave fullSave = new FullSave();
        //Save Backup1 = null;

        while (!leave)
        {
<<<<<<< HEAD
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
=======
            string response = View.ShowMenu();

            try
            {
                switch (response)
                {
                    case "1":
                        Save newSave = View.CreateBackupView();
                        Save addedSave = saveRepository.AjouterSave(newSave);
                        View.SaveAddedMessageView(addedSave);
                        Console.WriteLine(Language.GetString("PressAnyKey"));
                        Console.ReadLine();
                        break;

                    case "2":
                        Console.WriteLine(Language.GetString("BackupStarted"));
                        break;

                    case "3":
                        Console.WriteLine(Language.GetString("ViewLogs"));
                        Logger.Log("Backup1", @"C:\Source\File.txt", @"D:\Backup\File.txt", 1024, 500);
                        break;
>>>>>>> feature-SaveRepository

                    case "4":
                        Language.SetLanguage(View.GetLanguageChoice());
                        break;

                    case "5":
                        List<Save> saves = saveRepository.ObtenirToutesLesSaves();
                        if (saveRepository.EstVide())
                        {
                            View.NoBackupView();
                        }
                        else
                        {
                            View.AfficherSavesView(saves);
                        }

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
            catch (ReturnToMenuException ex)
            {
                // Handle the localized message from the exception
                Console.WriteLine(ex.Message);
                continue;
            }

            Console.Clear();
        }
    }
}


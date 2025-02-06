using System;

class Controler
{
    static void Main(string[] args)
    {
        SaveRepository saveRepository = new SaveRepository();
        bool leave = false;

        //Exemple de création d'une save
        FullSave fullSave = new FullSave();

        while (!leave)
        {
            string response = View.ShowMenu();
            try {
                switch (response)
                {
                    case "1":
                        Save newSave = View.CreateBackupView();
                        Save addedSave = saveRepository.AddSave(newSave);
                        View.SaveAddedMessageView(addedSave);
                        Console.WriteLine(Language.GetString("Controller_PressAnyKey"));
                        Console.ReadLine();
                        break;

                    case "2":
                        Console.WriteLine(Language.GetString("Controller_BackupStarted"));
                        break;

                    case "3":
                        Save save = new Save
                        {
                            name = "Backup1",
                            sourceDirectory = @"C:\Source\File.txt",
                            targetDirectory = @"D:\Backup\File.txt",
                            saveStrategy = new FullSave()
                        };
                        Console.WriteLine(Language.GetString("ViewLogs"));
                        Log.GeneralLog(save, 1024, 500);
                        break;

                    case "4":
                        // Change the language with the model
                        Language.SetLanguage(View.GetLanguageChoice());
                        break;

                    case "5":
                        List<Save> saves = saveRepository.GetAllSaves();
                        if (saveRepository.IsEmpty())
                        {
                            View.NoBackupView();
                        }
                        else
                        {
                            View.ShowSavesView(saves);
                        }
                        Console.WriteLine(Language.GetString("Controller_PressAnyKey"));
                        Console.ReadLine();
                        break;

                    case "6":
                        leave = true;
                        break;

                    default:
                        Console.WriteLine(Language.GetString("Controller_InvalidChoice"));
                        break;
                }
            }
            catch (ReturnToMenuException ex)
            {
                // Handle the localized message from the exception
                Console.WriteLine(ex.Message);
                continue;
            }
        }
    }
}

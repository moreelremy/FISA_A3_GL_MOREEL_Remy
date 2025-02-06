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
                        // Check if the save was successfully added
                        if (addedSave != null)
                        {
                            View.SaveAddedMessageView(addedSave);
                        }
                        else
                        {
                            // Display a message if the save limit is reached
                            Console.WriteLine(Language.GetString("Controller_MaxSaveLimitReached"));
                        }
                        Console.WriteLine(Language.GetString("Controller_PressAnyKey"));
                        Console.ReadLine();
                        break;

                    case "2":
                        Console.WriteLine(Language.GetString("Controller_BackupStarted"));
                        break;

                    case "3":
                        Console.WriteLine(Language.GetString("ControllerView_ViewLogs"));
                        Console.WriteLine(Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs"));
                        Logger.Log("Backup1", @"C:\Source\File.txt", @"D:\Backup\File.txt", 1024, 500);
                        Console.WriteLine(Language.GetString("Controller_ActualRepo") + $" : {Directory.GetCurrentDirectory()}");
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
            Console.Clear();
        }
    }
}

using System;
using System.Reflection;

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
                        /*
                         * permet de tester les logs 
                        for (int i = 0; i < 1111; i++)
                        {
                            Save save = new Save
                            {
                                name = "Backup1",
                                sourceDirectory = @"C:\Source\File.txt",
                                targetDirectory = @"D:\Backup\File.txt",
                                saveStrategy = new FullSave()
                            };
                            Logs.GeneralLog(save, 1024, 500);
                        }*/
                        Console.WriteLine(Language.GetString("ControllerView_ViewLogs"));
                        string wantedDate = View.GetWantedDate();
                        string filePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs", wantedDate + ".json"));

                        if (!File.Exists(filePath))
                        {
                            View.FileNotFound();
                            break;
                        }
                        

                        List<string> logLines = Logs.ReadGeneralLog(filePath);
                        if (logLines.Count >= 10)
                        {

                            for (int j = 0; j < 10; j++)
                            {
                                Console.WriteLine(logLines[j]);
                            }

                            for (int i = 10; i < logLines.Count; i++)
                            {
                                string? output = "";
                                while (string.IsNullOrEmpty(output))
                                {
                                    Console.WriteLine(logLines[i]);
                                    //Waits for a key to be pressed
                                    output = Console.ReadKey(true).KeyChar.ToString();
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < logLines.Count; j++)
                            {
                                Console.WriteLine(logLines[j]);
                            }
                            Console.ReadLine();
                        }


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
                        List<Save> SavesToDelete = saveRepository.GetAllSaves();
                        // Check if there are any saves to delete
                        if (saveRepository.IsEmpty())
                        {
                            View.NoBackupView();
                        }
                        else
                        {
                            // Display saves and get user input
                            View.DisplaySavesForDeletion(SavesToDelete);
                            int saveIndex = View.GetSaveIndexForDeletion(SavesToDelete.Count);

                            if (saveIndex != -1)
                            {
                                bool isDeleted = saveRepository.RemoveSaveByIndex(saveIndex);
                                View.DisplayDeleteResult(isDeleted);
                            }
                        }

                        Console.WriteLine(Language.GetString("Controller_PressAnyKey"));
                        Console.ReadLine();
                        break;



                    case "7":
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

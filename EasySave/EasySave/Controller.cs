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
                            View.Output(Language.GetString("Controller_MaxSaveLimitReached"));
                        }
                        View.Output(Language.GetString("Controller_PressAnyKey"));
                        Console.ReadLine();
                        break;

                    case "2":
                        List<Save> savesToExecute = saveRepository.GetAllSaves();

                        if (savesToExecute.Count == 0)
                        {
                            View.NoBackupView();
                        }
                        else
                        {
                            View.DisplaySavesForExecution(savesToExecute);
                            List<int> selectedIndexes = View.GetSaveSelection(savesToExecute.Count);

                            foreach (int index in selectedIndexes)
                            {
                                Save save = savesToExecute[index];
                                bool success = saveRepository.ExecuteSave(save);
                                View.DisplayExecutionResult(success);
                            }
                        }

                        Console.ReadLine();
                        break;

                    case "3":
                        List<Save> saves = saveRepository.GetAllSaves();
                        if (saveRepository.IsEmpty())
                        {
                            View.Output(Language.GetString("View_NoBackups"));
                        }
                        else
                        {
                            View.ShowSavesView(saves);
                            string choice = View.ShowChoiceMenuOrDelete();

                            switch (choice)
                            {
                                case "1":
                                    break;
                                case "2":
                                    //string ChoiceDelete = InputHelper.ReadLineNotNull(Language.GetString("Controller_AskChoiceDelete");
                                    // Display saves and get user input
                                    View.DisplaySavesForDeletion(saves);
                                    int saveIndex = View.GetSaveIndexForDeletion(saves.Count);

                                    if (saveIndex != -1)
                                    {
                                        bool isDeleted = saveRepository.RemoveSaveByIndex(saveIndex);
                                        View.DisplayDeleteResult(isDeleted);
                                    }
                                    break;
                            }
                        }
                        View.Output(Language.GetString("Controller_PressAnyKey"));
                        Console.ReadLine();
                        break;
                    
                    case "4":
                        
                         /*
                        for (int i = 0; i < 1111; i++)
                        {
                            Save save = new Save
                            {
                                name = "Backup1",
                                sourceDirectory = @"C:\Source\File.txt",
                                targetDirectory = @"D:\Backup\File.txt",
                                saveStrategy = new FullSave()
                            };
                            Logs.GeneralLog(save, 10,10);
                        }*/
                        View.Output(Language.GetString("ControllerView_ViewLogs"));
                        string wantedDate = View.GetWantedDate();
                        string filePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs/Logs", wantedDate + ".json"));

                        if (!File.Exists(filePath))
                        {
                            View.Output(Language.GetString("View_FileNotFound"));
                            break;
                        }

                        List<string> logLines = Logs.ReadGeneralLog(filePath);
                        if (logLines.Count >= 10)
                        {

                            for (int j = 0; j < 10; j++)
                            {
                                View.Output(logLines[j]);
                            }

                            for (int i = 10; i < logLines.Count; i++)
                            {
                                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                                if (keyInfo.Key != ConsoleKey.Enter)
                                {
                                    break;
                                }
                                else
                                {
                                    View.Output(logLines[i]);
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < logLines.Count; j++)
                            {
                                View.Output(logLines[j]);
                            }
                            Console.ReadLine();
                        }
                        break;

                    case "5":
                        // Change the language with the model
                        Language.SetLanguage(View.GetLanguageChoice());
                        break;

                    case "6":
                        leave = true;
                        break;

                    default:
                        View.Output(Language.GetString("Controller_InvalidChoice"));
                        break;
                }
            }
            catch (ReturnToMenuException ex)
            {
                // Handle the localized message from the exception
                View.Output(ex.Message);
                continue;
            }
        }
    }
}

using System;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using static Logs;

class Controller
{
    static void Main(string[] args)
    {
        SaveRepository saveRepository = new SaveRepository();

        FullSave fullSave = new FullSave();
        DifferentialSave differentialSave = new DifferentialSave();
        
        string repositoryStatePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../../RepositoryState.json");
        if (File.Exists(repositoryStatePath))
        {
            using JsonDocument repositoryState = JsonDocument.Parse(File.ReadAllText(repositoryStatePath));
            foreach (JsonElement saveState in repositoryState.RootElement.EnumerateArray())
            {
                saveRepository.AddSave(
                   new Save
                   {
                       name = saveState.GetProperty("name").GetString(),
                       sourceDirectory = saveState.GetProperty("sourceDirectory").GetString(),
                       targetDirectory = saveState.GetProperty("targetDirectory").GetString(),
                       saveStrategy = saveState.GetProperty("saveStrategy").GetString() == "FullSave" ? fullSave : differentialSave
                   }
               );
            }
        }

        while (true)
        {
            string response = View.ShowMenu();
            try {
                switch (response)
                {
                    case "1":
                        if (saveRepository.GetAllSaves().Count >= 5)
                        {
                            View.Output(Language.GetString("Controller_MaxSaveLimitReached"));  // Indicate that the save was not added Display a message if the save limit is reached
                        }
                        else
                        {
                            Save newSave = View.CreateBackupView();
                            saveRepository.AddSave(newSave);
                            // Check if the save was successfully added
                            View.SaveAddedMessageView(newSave);
                        }
                        View.PromptToContinue();
                        break;

                    case "2":
                        if (saveRepository.IsEmpty())
                        {
                            View.NoBackupView();
                        }
                        else
                        {
                            List<Save> savesToExecute = saveRepository.GetAllSaves();
                            View.DisplaySavesForExecution(savesToExecute);

                            int saveIndex = View.GetSaveIndexForExecution(savesToExecute.Count);
                            if (saveIndex != -1)
                            {
                                string errorMessage;
                                bool success = saveRepository.ExecuteSave(savesToExecute[saveIndex], out errorMessage);

                                if (success)
                                {
                                    View.DisplaySuccess(Language.GetString("View_ExecutionCompleted"));
                                }
                                else
                                {
                                    View.DisplayError(errorMessage);
                                }
                            }
                        }
                        View.PromptToContinue();
                        break;

                    case "3":
                        if (saveRepository.IsEmpty())
                        {
                            View.Output(Language.GetString("View_NoBackups"));
                        }
                        else
                        {
                            List<Save> saves = saveRepository.GetAllSaves();
                            View.ShowSavesView(saves);
                            string choice;
                            while (true)
                            {
                                choice = View.ShowChoiceMenuOrDelete();
                                if (choice == "1" || choice == "2")
                                {
                                    break;
                                }
                                else
                                {
                                    View.Output(Language.GetString("Controller_InvalidChoice"));
                                }
                            }

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
                        View.PromptToContinue();
                        break;
                    
                    case "4":
                        View.Output(Language.GetString("ControllerView_ViewLogs"));
                        string wantedDate = View.GetWantedDate();
                        string filePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs/Logs", wantedDate + ".json"));

                        if (!File.Exists(filePath))
                        {
                            View.Output(Language.GetString("View_FileNotFound"));
                            View.PromptToContinue();
                            break;
                        }

                        List<LogEntry> logLines = Logs.ReadGeneralLog(filePath);
                        if (logLines.Count >= 10)
                        {

                            for (int j = 0; j < 10; j++)
                            {
                                View.DisplayLog(logLines[j]);
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
                                    View.DisplayLog(logLines[i]);
                                }
                            }
                            View.PromptToContinue();
                        }
                        else
                        {
                            for (int j = 0; j < logLines.Count; j++)
                            {
                                    View.DisplayLog(logLines[j]);
                            }
                            View.PromptToContinue();
                        }
                        break;
                    
                    case "5":
                        // Change the language with the model
                        Language.SetLanguage(View.GetLanguageChoice());
                        break;

                    case "6":
                        string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "../../../../RepositoryState.json");
                        if (File.Exists(pathFile))
                        {
                            File.Delete(pathFile);
                        }
                        Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
                        List<dynamic> savesSates = new List<dynamic>();
                        foreach (Save save in saveRepository.GetAllSaves()) {  
                            string saveStrategy = save.saveStrategy.GetType().Name;
                            string jsonEntry = JsonSerializer.Serialize(save);
                            jsonEntry = jsonEntry.Replace("{}", $"\"{saveStrategy}\"");
                            savesSates.Add(JsonSerializer.Deserialize<dynamic>(jsonEntry));
                        };
                        string repositoryState = JsonSerializer.Serialize(savesSates, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(pathFile, repositoryState);
                        View.PromptToContinue();
                        Environment.Exit(0);
                        break;

                    default:
                        View.Output(Language.GetString("Controller_InvalidChoice"));
                        View.PromptToContinue();
                        break;
                }
            }
            catch (ReturnToMenuException ex)
            {
                // Handle the localized message from the exception
                View.Output(ex.Message);
                View.PromptToContinue();
            }
            Console.Clear();
        }
    }
}
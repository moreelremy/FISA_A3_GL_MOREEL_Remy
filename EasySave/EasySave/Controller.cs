using System;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;

class Controler
{
    static void Main(string[] args)
    {
        SaveRepository saveRepository = new SaveRepository();
        bool leave = false;

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
                        View.Output(Language.GetString("Controller_BackupStarted"));
                        break;

                    case "3":
                        View.Output(Language.GetString("ControllerView_ViewLogs"));
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
                                View.Output(logLines[j]);
                            }

                            for (int i = 10; i < logLines.Count; i++)
                            {
                                string? output = "";
                                while (string.IsNullOrEmpty(output))
                                {
                                    View.Output(logLines[i]);
                                    //Waits for a key to be pressed
                                    output = Console.ReadKey(true).KeyChar.ToString();
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
                        View.Output(Language.GetString("Controller_PressAnyKey"));
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

                        View.Output(Language.GetString("Controller_PressAnyKey"));
                        Console.ReadLine();
                        break;

                    case "7":
                        var options = new JsonSerializerOptions{ WriteIndented = true };
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
                        string repositoryState = JsonSerializer.Serialize(savesSates, options);
                        File.WriteAllText(pathFile, repositoryState);
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

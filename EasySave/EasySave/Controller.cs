using System.Text.Json;
using System.Text.RegularExpressions;
using SettingsModel;

class Controller
{
    static void Main(string[] args)
    {
        IView objView;
        switch (args[0])
        {
            case "Console":
            default:
                objView = new ViewBasic();
                break;

            case "Server":
                objView = new ViewServer();
                break;
        }

        SaveRepository saveRepository = new SaveRepository();
        SaveStrategyFactory saveStrategyFactory = new SaveStrategyFactory();

        string repositoryStatePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../../RepositoryState.json");
        if (File.Exists(repositoryStatePath))
        {
            using JsonDocument repositoryState = JsonDocument.Parse(File.ReadAllText(repositoryStatePath));
            foreach (JsonElement saveState in repositoryState.RootElement.EnumerateArray())
            {
                string name = saveState.TryGetProperty("name", out JsonElement nameElement) ? nameElement.GetString() : "DefaultName";
                string sourceDirectory = saveState.TryGetProperty("sourceDirectory", out JsonElement sourceElement) ? sourceElement.GetString() : "DefaultSource";
                string targetDirectory = saveState.TryGetProperty("targetDirectory", out JsonElement targetElement) ? targetElement.GetString() : "DefaultTarget";
                string saveStrategy = saveState.TryGetProperty("saveStrategy", out JsonElement strategyElement) ? strategyElement.GetString() : "FullStrategy";
                string logFileExtension = saveState.TryGetProperty("logFileExtension", out JsonElement logElement) ? logElement.GetString() : "json";
                saveRepository.AddSave(new Save
                {
                    name = name,
                    sourceDirectory = sourceDirectory,
                    targetDirectory = targetDirectory,
                    saveStrategy = saveStrategyFactory.CreateSaveStrategy(saveStrategy),
                    logFileExtension = logFileExtension
                });
            }
        }

        while (true)
        {
            string response = objView.ShowMenu();
            try
            {
                switch (response)
                {
                    case "1":
                        if (saveRepository.GetAllSaves().Count >= 5)
                        {
                            objView.Output(Language.GetString("Controller_MaxSaveLimitReached"));  // Indicate that the save was not added Display a message if the save limit is reached
                        }
                        else
                        {
                            try
                            {
                                Dictionary<string, string> dictSave = objView.CreateBackupView();
                                Save newSave = new Save
                                {
                                    name = dictSave["name"],
                                    sourceDirectory = dictSave["sourceDirectory"],
                                    targetDirectory = dictSave["targetDirectory"],
                                    saveStrategy = saveStrategyFactory.CreateSaveStrategy(dictSave["saveStrategy"]),
                                    logFileExtension = dictSave["logFileExtension"]
                                };
                                saveRepository.AddSave(newSave);
                                // Check if the save was successfully added
                                objView.SaveAddedMessageView(newSave);
                            }
                            catch (Exception ex)
                            {
                                objView.Output(ex.Message);

                            }
                        } 
                        objView.PromptToContinue();
                        break;

                    case "2":
                        if (saveRepository.IsEmpty())
                        {
                            objView.Output(Language.GetString("View_NoBackups"));
                        }
                        else
                        {
                            List<Save> savesToExecute = saveRepository.GetAllSaves();
                            objView.DisplaySavesForExecution(savesToExecute);

                            List<int> saveIndexes = objView.GetSaveSelection(savesToExecute.Count);
                            if (saveIndexes.Count > 0)
                            {
                                foreach (int index in saveIndexes)
                                {
                                    string errorMessage;
                                    bool success = saveRepository.ExecuteSave(savesToExecute[index], new CancellationToken(), new ManualResetEventSlim(true),
                                    progress => {/* no-op, or log progress if needed */ }, out errorMessage);

                                    if (success)
                                    {
                                        objView.Output(Language.GetString("View_ExecutionCompleted"));
                                    }
                                    else
                                    {
                                        objView.Output(errorMessage);
                                    }
                                }
                            }
                        }
                        objView.PromptToContinue();
                        break;

                    case "3":
                        if (saveRepository.IsEmpty())
                        {
                            objView.Output(Language.GetString("View_NoBackups"));
                        }
                        else
                        {
                            List<Save> saves = saveRepository.GetAllSaves();
                            objView.ShowSavesView(saves);
                            string choice;
                            while (true)
                            {
                                choice = objView.ShowChoiceMenuOrDelete();
                                if (choice == "1" || choice == "2")
                                {
                                    break;
                                }
                                else
                                {
                                    objView.Output(Language.GetString("Controller_InvalidChoice"));
                                }
                            }

                            switch (choice)
                            {
                                case "1":
                                    break;
                                case "2":
                                    objView.DisplaySavesForDeletion(saves);
                                    int saveIndex = objView.GetSaveIndexForDeletion(saves.Count);

                                    if (saveIndex != -1)
                                    {
                                        bool isDeleted = saveRepository.RemoveSaveByIndex(saveIndex);
                                        objView.DisplayDeleteResult(isDeleted);
                                    }
                                    break;
                            }
                        }
                        objView.PromptToContinue();
                        break;

                    case "4":

                        objView.Output(Language.GetString("ControllerView_ViewLogs"));
                        string wantedDate = objView.GetWantedDate();
                        if (!Regex.IsMatch(wantedDate, "^(0[1-9]|[12][0-9]|3[01])-(0[1-9]|1[0-2])-\\d{4}$"))
                        {
                            objView.Output(Language.GetString("View_FormatDontMatch"));
                            objView.PromptToContinue();
                            break;
                        }
                        IEnumerable<string> files;
                        try
                        {
                            files = Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs/Logs"), wantedDate + ".*");
                        }
                        catch
                        {
                            files = Enumerable.Empty<string>();

                        }

                        if (!files.Any())
                        {
                            objView.Output(Language.GetString("View_FileNotFound"));
                            objView.PromptToContinue();
                            break;
                        }

                        List<Dictionary<string, object>> logLines = Data.ReadGeneralLog(files);
                        if (logLines.Count >= 10)
                        {

                            for (int j = 0; j < 10; j++)
                            {
                                objView.DisplayLog(logLines[j]);
                            }

                            for (int i = 10; i < logLines.Count; i++)
                            {
                                objView.DisplayLog(logLines[i]);
                                objView.Input(LineNotNull: false);
                            }
                            objView.PromptToContinue();
                        }
                        else
                        {
                            for (int j = 0; j < logLines.Count; j++)
                            {
                                objView.DisplayLog(logLines[j]);
                            }
                            objView.PromptToContinue();
                        }
                        break;

                    case "5":

                        bool continueInSettings = true;

                        while (continueInSettings)
                        {
                            SettingsConsole appSettings = new SettingsConsole();
                            appSettings.LoadSettings();
                            Console.Clear();

                            // Afficher les paramètres actuels avec un menu de modification
                            objView.DisplaySettingsMenu(appSettings);

                            int parameterChoice = -1;

                            // Demander à l'utilisateur de choisir un paramètre à modifier
                            while (parameterChoice < 1 || parameterChoice > 4)
                            {
                                objView.Output(Language.GetString("Controller_AskSettings") + " : ");
                                string input = objView.Input();
                                if (int.TryParse(input, out parameterChoice) && parameterChoice >= 1 && parameterChoice <= 4)
                                {
                                    break;
                                }
                                objView.Output(Language.GetString("Controller_SettingIncorrect"));
                            }

                            // Modifier le paramètre choisi
                            switch (parameterChoice)
                            {
                                case 1:
                                    objView.Output(Language.GetString("Controller_EnterSoftware") + " : ");
                                    appSettings.SettingsSoftware = objView.Input();
                                    break;
                                case 2:
                                    objView.Output(Language.GetString("Controller_EnterExtensionCrypt") + " : ");
                                    string newExtensionsToCrypt = objView.Input();
                                    appSettings.ExtensionsToCrypt = appSettings.ParseExtensions(newExtensionsToCrypt);
                                    break;
                                case 3:
                                    objView.Output(Language.GetString("Controller_EnterExtensionPrio") + " : ");
                                    string newExtensionToPrioritize = objView.Input();
                                    appSettings.ExtensionsToPrioritize = appSettings.ParseExtensions(newExtensionToPrioritize);
                                    break;
                                case 4:
                                    objView.Output(Language.GetString("Controller_EnterLimitKo") + " : ");
                                    string inputSaturation = objView.Input();
                                    if (int.TryParse(inputSaturation, out int value))
                                    {
                                        appSettings.SettingSaturationLimit = value;
                                    }
                                    else
                                    {
                                        objView.Output(Language.GetString("Controller_SaturationString"));
                                    }
                                    break;
                            }

                            // Sauvegarder les paramètres modifiés (si une modification a eu lieu)
                            appSettings.SaveSettings();
                            objView.Output(Language.GetString("Controller_SettingsUpdated"));
                            Thread.Sleep(1000);
                        }

                        break;

                    case "6":
                        // Change the language with the model
                        Language.SetLanguage(objView.GetLanguageChoice());
                        break;

                    case "7":
                        string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "../../../../RepositoryState.json");
                        if (File.Exists(pathFile))
                        {
                            File.Delete(pathFile);
                        }
                        Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
                        List<dynamic> savesSates = new List<dynamic>();
                        foreach (Save save in saveRepository.GetAllSaves())
                        {
                            string saveStrategy = save.saveStrategy.GetType().Name;
                            string jsonEntry = JsonSerializer.Serialize(save);
                            jsonEntry = jsonEntry.Replace("{}", $"\"{saveStrategy}\"");
                            savesSates.Add(JsonSerializer.Deserialize<dynamic>(jsonEntry));
                        }
                        ;
                        string repositoryState = JsonSerializer.Serialize(savesSates, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(pathFile, repositoryState);
                        objView.PromptToContinue();
                        Environment.Exit(0);
                        break;

                    default:
                        objView.Output(Language.GetString("Controller_InvalidChoice"));
                        objView.PromptToContinue();
                        break;
                }
            }
            catch (ReturnToMenuException ex)
            {
                // Handle the localized message from the exception
                objView.Output(ex.Message);
                objView.PromptToContinue();
            }
        }
    }
}
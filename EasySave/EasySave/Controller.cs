﻿using System.Text.Json;
using System.Text.RegularExpressions;
using EasySaveConsole;

class Controller
{
    static void Main(string[] args)
    {
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
        IView objView = new ViewBasic();
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
                                    bool success = saveRepository.ExecuteSave(savesToExecute[index], out errorMessage);

                                    if (success)
                                    {
                                        objView.DisplaySuccess(Language.GetString("View_ExecutionCompleted"));
                                    }
                                    else
                                    {
                                        objView.DisplayError(errorMessage);
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
                                    //string ChoiceDelete = InputHelper.ReadLineNotNull(Language.GetString("Controller_AskChoiceDelete");
                                    // Display saves and get user input
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

                                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                                if (keyInfo.Key != ConsoleKey.Enter)
                                {
                                    break;
                                }
                                else
                                {
                                    objView.DisplayLog(logLines[i]);
                                }
                            }
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
                            Settings appSettings = Settings.LoadSettings();
                            Console.Clear();
                            // Afficher les paramètres actuels avec un menu de modification
                            objView.DisplaySettingsMenu(appSettings);

                            int parameterChoice = -1;

                            // Demander à l'utilisateur de choisir un paramètre à modifier
                            while (parameterChoice < 1 || parameterChoice > 4)
                            {
                                string input = InputHelper.ReadLineNotNull(Language.GetString("Controller_AskSettings") +" : ");
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
                                    appSettings.UserInputSettingsSoftware = InputHelper.ReadLineNotNull(Language.GetString("Controller_EnterSoftware")+" : ");
                                    break;
                                case 2:
                                    string newExtensions = InputHelper.ReadLineNotNull(Language.GetString("Controller_EnterExtensionCrypt")+" : ");
                                    appSettings.ExtensionSelected = newExtensions.Split(',').ToList();
                                    break;
                                case 3:
                                    appSettings.ExtensionToPrioritize = InputHelper.ReadLineNotNull(Language.GetString("Controller_EnterExtensionPrio")+" : ");
                                    break;
                                case 4:
                                    appSettings.SettingSaturationLimit = InputHelper.ReadLineNotNull(Language.GetString("Controller_EnterLimitKo")+" : ");
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
                        };
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
using EasySaveConsole;

public interface IView
{
    string ShowMenu();
    string GetLanguageChoice();
    void SaveAddedMessageView(Save save);
    void ShowSavesView(List<Save> saves);
    void DisplaySavesForExecution(List<Save> saves);
    List<int> GetSaveSelection(int maxCount);
    void DisplayExecutionResult(bool success);
    string ShowChoiceMenuOrDelete();
    void DisplaySavesForDeletion(List<Save> saves);
    int GetSaveIndexForDeletion(int maxIndex);
    void DisplayDeleteResult(bool isDeleted);
    void DisplaySuccess(string message);
    void DisplayError(string message);
    Dictionary<string, string> CreateBackupView();
    string GetWantedDate();
    void PromptToContinue();
    void Output(string output);
    void DisplayLog(Dictionary<string, object> log);

    void DisplaySettingsMenu(Settings appSettings);
}

/// <summary>
/// Manages View
/// </summary>
class ViewBasic : IView
{
    /// <summary>
    /// Displays the menu (choice for the user)
    /// </summary>
    /// <returns>The number entered by the user</returns>
    public string ShowMenu()
    {
        Console.WriteLine("╔═════════════════════════════════════╗");
        Console.WriteLine("║              Easy Save              ║");
        Console.WriteLine("╚═════════════════════════════════════╝");
        Console.WriteLine($"    [1]: {Language.GetString("View_CreateBackup")}");
        Console.WriteLine($"    [2]: {Language.GetString("View_StartBackup")}");
        Console.WriteLine($"    [3]: {Language.GetString("View_ViewAllSaves")}");
        Console.WriteLine($"    [4]: {Language.GetString("ControllerView_ViewLogs")}");
        Console.WriteLine($"    [5]: {Language.GetString("WPF_SettingTitle")}");
        Console.WriteLine($"    [6]: {Language.GetString("View_ChangeLanguage")}");
        Console.WriteLine($"    [7]: {Language.GetString("View_ExitApp")}\n\n");

        return InputHelper.ReadLineNotNull(Language.GetString("View_EnterNumber"), allowReturnToMenu: false);
    }

    /// <summary>
    /// Ask the user to choose a language
    /// </summary>
    /// <returns>The selected language code (EN / FR / ..)</returns>
    public string GetLanguageChoice()
    {
        return InputHelper.ReadLineNotNull(Language.GetString("View_LanguageChoice"));
    }

    /// <summary>
    /// Displays that the backup has been created and takes the list as a parameter
    /// </summary>
    /// <param name="save">Set up a backup list</param>
    public void SaveAddedMessageView(Save save)
    {
        Console.WriteLine(string.Format(Language.GetString("View_SaveAddedMessage"), save.name));
    }

    /// <summary>
    /// Displays the list of backups with their number in the list
    /// </summary>
    /// <param name="saves">Set up a backup list</param>
    public void ShowSavesView(List<Save> saves)
    {
        Console.WriteLine("╔═══════════════════════════════════╗");
        Console.WriteLine(Language.GetString("View_ListOfBackups"));
        Console.WriteLine("╚═══════════════════════════════════╝");
        for (int i = 0; i < saves.Count; i++)
        {
            Console.WriteLine($"   {Language.GetString("View_NumberSave")} : [{i + 1}]");
            Console.WriteLine($"   " + Language.GetString("View_SaveName") + $" : {saves[i].name}");
            Console.WriteLine($"   " + Language.GetString("View_SaveSource") + $" : {saves[i].sourceDirectory}");
            Console.WriteLine($"   " + Language.GetString("View_SaveTarget") + $" : {saves[i].targetDirectory}");
            Console.WriteLine($"   " + Language.GetString("View_SaveType") + $" : {Language.GetString($"View_{saves[i].saveStrategy.GetType().Name}")}");
            Console.WriteLine($"   " + Language.GetString("View_logFileExtension") + $" : {saves[i].logFileExtension}");
            Console.WriteLine("═════════════════════════════════════");
        }

    }

    public void DisplaySavesForExecution(List<Save> saves)
    {
        Console.WriteLine(Language.GetString("View_ChooseSaveToExecute"));
        for (int i = 0; i < saves.Count; i++)
        {
            Console.WriteLine($"[{i + 1}] {saves[i].name}");
        }
    }

    /// <summary>
    /// Parses a user input string representing selected indices or index ranges and converts them into a list of zero-based indices.
    /// </summary>
    /// <param name="input">A string containing individual indices or ranges separated by semicolons (e.g., "1;3-5;7").</param>
    /// <param name="maxCount">The maximum allowed index (1-based), ensuring selections stay within valid bounds.</param>
    /// <returns>
    /// A list of integers representing the zero-based indices of the selected items.
    /// If the input is invalid, an empty list is returned.
    /// </returns>
    private List<int> ParseSaveSelection(string input, int maxCount)
    {
        List<int> selectedIndexes = new List<int>();

        try
        {
            string[] ranges = input.Split(';');
            foreach (string range in ranges)
            {
                if (range.Contains('-'))
                {
                    string[] bounds = range.Split('-');
                    int start = int.Parse(bounds[0]) - 1;
                    int end = int.Parse(bounds[1]) - 1;

                    for (int i = start; i <= end && i < maxCount; i++)
                    {
                        if (!selectedIndexes.Contains(i))
                            selectedIndexes.Add(i);
                    }
                }
                else
                {
                    int index = int.Parse(range) - 1;
                    if (index >= 0 && index < maxCount)
                    {
                        selectedIndexes.Add(index);
                    }
                }
            }
        }
        catch
        {
            Console.WriteLine(Language.GetString("View_InvalidSelection"));
        }

        return selectedIndexes;
    }

    /// <summary>
    /// Prompts the user to enter save IDs to execute and parses them into a list of zero-based indices.
    /// </summary>
    /// <param name="maxCount">The maximum number of selectable save entries.</param>
    /// <returns>A list of integers representing the selected zero-based indices.</returns>
    public List<int> GetSaveSelection(int maxCount)
    {
        string input = InputHelper.ReadLineNotNull(Language.GetString("View_EnterSaveIdsToExecute"));
        return ParseSaveSelection(input, maxCount);
    }

    /// <summary>
    /// Displays the execution result message based on success or failure.
    /// </summary>
    /// <param name="success">A boolean indicating whether the execution was successful.</param>
    public void DisplayExecutionResult(bool success)
    {
        Console.WriteLine(success ? Language.GetString("View_ExecutionCompleted") : Language.GetString("View_ExecutionFailed"));
    }

    /// <summary>
    /// Shows the choice between returning to the menu or deleting a save
    /// </summary>
    /// <returns>returns the user's choice to use it in the switch case</returns>
    public string ShowChoiceMenuOrDelete()
    {
        string choice = InputHelper.ReadLineNotNull(
                        Language.GetString("Controller_ChoiceDeleteOrMenu") + "\n\n" +
                        "[1] : " + Language.GetString("Controller_ChoiceMenu") + "\n" +
                        "[2] : " + Language.GetString("Controller_ChoiceDelete"));
        return choice;
    }

    /// <summary>
    /// Displays a list of saves with corresponding numbers.
    /// </summary>
    /// <param name="saves">List of saves to display.</param>
    public void DisplaySavesForDeletion(List<Save> saves)
    {
        Console.WriteLine(Language.GetString("View_ChooseSaveToDelete"));

        for (int i = 0; i < saves.Count; i++)
        {
            Console.WriteLine($"[{i + 1}] {saves[i].name}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Reads the user's choice of save to delete.
    /// </summary>
    /// <returns>The chosen save index (0-based).</returns>
    public int GetSaveIndexForDeletion(int maxIndex)
    {
        string input = InputHelper.ReadLineNotNull(Language.GetString("View_EnterSaveNumber"));
        if (int.TryParse(input, out int index) && index > 0 && index <= maxIndex)
        {
            return index - 1;  // Convert to 0-based index
        }

        Console.WriteLine(Language.GetString("Controller_InvalidChoice"));
        return -1;  // Invalid choice
    }

  


    /// <summary>
    /// Displays the result of the deletion.
    /// </summary>
    /// <param name="isDeleted">True if the save was deleted, otherwise false.</param>
    public void DisplayDeleteResult(bool isDeleted)
    {
        if (isDeleted)
        {
            Console.WriteLine(Language.GetString("View_SaveDeleted"));
        }
        else
        {
            Console.WriteLine(Language.GetString("View_SaveNotFound"));
        }
    }

    /// <summary>
    /// Displays a success message in green.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public void DisplaySuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    /// <summary>
    /// Displays an error message in red.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public void DisplayError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    /// <summary>
    /// Collects information from the user to create a new backup, With a return to menu option
    /// </summary>
    /// <returns>The newly created Save object</returns>
    public Dictionary<string, string> CreateBackupView()
    {
        string name = InputHelper.ReadLineNotNull(Language.GetString("View_EnterBackupName"));
        string source = InputHelper.ReadLineNotNull(Language.GetString("View_EnterSourcePath"));
        string target = InputHelper.ReadLineNotNull(Language.GetString("View_EnterTargetPath"));

        Console.WriteLine("[1] " + Language.GetString("View_FullSave"));
        Console.WriteLine("[2] " + Language.GetString("View_DifferentialSave"));
        string saveStrategy;
        while (true)
        {
            saveStrategy = InputHelper.ReadLineNotNull(Language.GetString("View_SelectBackupType"));
            if (saveStrategy == "1" || saveStrategy == "2")
            {
                break;
            }
            else
            {
                Console.WriteLine(Language.GetString("View_InvalidSelection")); // Display error message
            }
        }

        Console.WriteLine("[1] " + Language.GetString("View_jsonFileExtension"));
        Console.WriteLine("[2] " + Language.GetString("View_xmlFileExtension"));
        string logFileExtension;
        while (true)
        {
            logFileExtension = InputHelper.ReadLineNotNull(Language.GetString("View_SelectBackupType"));
            if (logFileExtension == "1" || logFileExtension == "2")
            {
                break;
            }
            else
            {
                Console.WriteLine(Language.GetString("View_InvalidSelection")); // Display error message
            }
        }

        switch (logFileExtension)
        {
            case "1":
                logFileExtension = "json";
                break;

            case "2":
                logFileExtension = "xml";
                break;

            default:
                logFileExtension = "json";
                break;
        }

        Dictionary<string, string> result = new Dictionary<string, string>();
        result.Add("name", name);
        result.Add("sourceDirectory", source);
        result.Add("targetDirectory", target);
        result.Add("saveStrategy", saveStrategy);
        result.Add("logFileExtension", logFileExtension);
        return result;
    }

    /// <summary>
    /// Ask the user to choose a date for listing logs
    /// </summary>
    /// <returns>The date dd-mm-yyyy</returns>
    public string GetWantedDate()
    {
        Console.WriteLine(Language.GetString("View_DateChoice"));
        string? result = Console.ReadLine();

        return result == "" ? $"{DateTime.Now:dd-MM-yyyy}" : result;
    }

    /// <summary>
    /// Prompts the user to press any key to continue.
    /// </summary>
    public void PromptToContinue()
    {
        Console.WriteLine(Language.GetString("Controller_PressAnyKey"));
        Console.ReadLine();
        Console.Clear();
    }

    /// <summary>
    /// Just print a message in the console
    /// </summary>
    /// <param name="output">Take the sentence to display as a parameter</param>
    public void Output(string output)
    {
        Console.WriteLine(output);
    }

    /// <summary>
    /// Affiche les informations d'un log dans la console.
    /// </summary>
    /// <param name="log">L'entrée de log à afficher.</param>
    public void DisplayLog(Dictionary<string, object> log)
    {
        if (log == null)
        {
            Console.WriteLine("LogEntry is null.");
            return;
        }

        Console.WriteLine($"Timestamp: {log["timestamp"]}");
        Console.WriteLine($"SaveName: {log["saveName"]}");
        Console.WriteLine($"Source: {log["source"]}");
        Console.WriteLine($"Target: {log["target"]}");
        Console.WriteLine($"Size: {log["size"]}");
        Console.WriteLine($"TransferTimeMs: {log["transferTimeMs"]}");
        Console.WriteLine("══════════════════════════════");
    }

    public void DisplaySettingsMenu(Settings appSettings)
    {
        Console.WriteLine(Language.GetString("WPF_SettingTitle") + "\n");
        Console.WriteLine("[1] " + Language.GetString("WPF_SettingSoftware") + " : " + appSettings.UserInputSettingsSoftware);
        Console.WriteLine("[2] " + Language.GetString("WPF_SettingExtensionToCrypt") + " : " + string.Join(", ", appSettings.ExtensionToCrypt));
        Console.WriteLine("[3] " + Language.GetString("WPF_SettingExtensionToPrioritize") + " : " + string.Join(", ", appSettings.ExtensionToPrioritize));
        Console.WriteLine("[4] " + Language.GetString("WPF_SettingSaturationLimit") + " : " + appSettings.SettingSaturationLimit);
    }


}
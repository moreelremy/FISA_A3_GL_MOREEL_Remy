using System.Resources;

/// <summary>
/// Manages View
/// </summary>
class View
{
    /// <summary>
    /// Displays the menu (choice for the user)
    /// </summary>
    /// <returns>The number entered by the user</returns>
    public static string ShowMenu()
    {
        Console.WriteLine("╔═════════════════════════════════════╗");
        Console.WriteLine("║              Easy Save              ║");
        Console.WriteLine("╚═════════════════════════════════════╝");
        Console.WriteLine($"    [1]: {Language.GetString("View_CreateBackup")}");
        Console.WriteLine($"    [2]: {Language.GetString("View_StartBackup")}");
        Console.WriteLine($"    [3]: {Language.GetString("View_ViewAllSaves")}");
        Console.WriteLine($"    [4]: {Language.GetString("ControllerView_ViewLogs")}");
        Console.WriteLine($"    [5]: {Language.GetString("View_ChangeLanguage")}");
        Console.WriteLine($"    [6]: {Language.GetString("View_ExitApp")}\n\n");

        return InputHelper.ReadLineNotNull(Language.GetString("View_EnterNumber"), allowReturnToMenu: false);
    }

    /// <summary>
    /// Ask the user to choose a language
    /// </summary>
    /// <returns>The selected language code (EN / FR / ..)</returns>
    public static string GetLanguageChoice()
    {
        return InputHelper.ReadLineNotNull(Language.GetString("View_LanguageChoice"));
    }

    /// <summary>
    /// Displays that the backup has been created and takes the list as a parameter
    /// </summary>
    /// <param name="save">Set up a backup list</param>
    public static void SaveAddedMessageView(Save save)
    {
        Console.WriteLine(string.Format(Language.GetString("View_SaveAddedMessage"), save.name));
    }

    /// <summary>
    /// Displays the list of backups with their number in the list
    /// </summary>
    /// <param name="saves">Set up a backup list</param>
    public static void ShowSavesView(List<Save> saves)
    {
        Console.WriteLine("╔═══════════════════════════════════╗");
        Console.WriteLine(Language.GetString("View_ListOfBackups"));
        Console.WriteLine("╚═══════════════════════════════════╝");
        for(int i = 0; i < saves.Count; i++)
        {
            Console.WriteLine($"   {Language.GetString("View_NumberSave")} : [{i + 1}]");
            Console.WriteLine($"   " + Language.GetString("View_SaveName") + $" : {saves[i].name}");
            Console.WriteLine($"   " + Language.GetString("View_SaveSource") + $" : {saves[i].sourceDirectory}");
            Console.WriteLine($"   " + Language.GetString("View_SaveTarget") + $" : {saves[i].targetDirectory}");
            Console.WriteLine($"   " + Language.GetString("View_SaveType") + $" : {(saves[i].saveStrategy is FullSave ? Language.GetString("View_FullSave") : Language.GetString("View_DifferentialSave"))}");
            Console.WriteLine("═════════════════════════════════════");
        }
        
    }
    public static void NoBackupView()
    {
        Console.WriteLine(Language.GetString("View_NoBackups"));
    }

    public static void DisplaySavesForExecution(List<Save> saves)
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
    private static List<int> ParseSaveSelection(string input, int maxCount)
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
    public static List<int> GetSaveSelection(int maxCount)
    {
        string input = InputHelper.ReadLineNotNull(Language.GetString("View_EnterSaveIdsToExecute"));
        return ParseSaveSelection(input, maxCount);
    }

    /// <summary>
    /// Displays the execution result message based on success or failure.
    /// </summary>
    /// <param name="success">A boolean indicating whether the execution was successful.</param>
    public static void DisplayExecutionResult(bool success)
    {
        Console.WriteLine(success ? Language.GetString("View_ExecutionCompleted") : Language.GetString("View_ExecutionFailed"));
    }


    /// <summary>
    /// Shows the choice between returning to the menu or deleting a save
    /// </summary>
    /// <returns>returns the user's choice to use it in the switch case</returns>
    public static string ShowChoiceMenuOrDelete()
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
    public static void DisplaySavesForDeletion(List<Save> saves)
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
    public static int GetSaveIndexForDeletion(int maxIndex)
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
    public static void DisplayDeleteResult(bool isDeleted)
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
    /// Collects information from the user to create a new backup, With a return to menu option
    /// </summary>
    /// <returns>The newly created Save object</returns>
    public static Save CreateBackupView()
    {
        string name = InputHelper.ReadLineNotNull(Language.GetString("View_EnterBackupName"));
        string source = InputHelper.ReadLineNotNull(Language.GetString("View_EnterSourcePath"));
        string target = InputHelper.ReadLineNotNull(Language.GetString("View_EnterTargetPath"));
        Console.WriteLine("[1] " + Language.GetString("View_FullSave"));
        Console.WriteLine("[2] " + Language.GetString("View_DifferentialSave"));

        string typeChoice;
        while (true)
        {
            typeChoice = InputHelper.ReadLineNotNull(Language.GetString("View_SelectBackupType"));
            if (typeChoice == "1" || typeChoice == "2")
            {
                break; 
            }
            else
            {
                Console.WriteLine(Language.GetString("View_InvalidBackupType")); // Display error message
            }
        }
        ISaveStrategy saveStrategy = typeChoice == "2" ? new DifferentialSave() : new FullSave();
        return new Save
        {
            name = name,
            sourceDirectory = source,
            targetDirectory = target,
            saveStrategy = saveStrategy,
        };
    }

    /// <summary>
    /// Ask the user to choose a date for listing logs
    /// </summary>
    /// <returns>The date dd-mm-yyyy</returns>
    public static string GetWantedDate()
    {
        return InputHelper.ReadLineNotNull(Language.GetString("View_DateChoice"));
    }

    /// <summary>
    /// Just print a message in the console
    /// </summary>
    /// <param name="output">Take the sentence to display as a parameter</param>
    public static void Output(string output)
    {
        Console.WriteLine(output);
    }
}

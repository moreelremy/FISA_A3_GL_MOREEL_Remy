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
        Console.WriteLine($"    [3]: {Language.GetString("ControlerView_ViewLogs")}");
        Console.WriteLine($"    [4]: {Language.GetString("View_ChangeLanguage")}");
        Console.WriteLine($"    [5]: {Language.GetString("View_ViewAllSaves")}");
        Console.WriteLine($"    [6]: {Language.GetString("View_RemoveSaves")}");
        Console.WriteLine($"    [7]: {Language.GetString("View_ExitApp")}\n\n");

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

    public static void SaveAddedMessageView(Save save)
    {
        Console.WriteLine(string.Format(Language.GetString("View_SaveAddedMessage"), save.name));
    }

    public static void ShowSavesView(List<Save> saves)
    {
        Console.WriteLine("╔════════════════════════════════════");
        Console.WriteLine("║ " + Language.GetString("View_ListOfBackups"));
        foreach (var save in saves)
        {
            Console.WriteLine("╠════════════════════════════════════");
            Console.WriteLine($"║ " + Language.GetString("View_SaveName") + $" : {save.name}");
            Console.WriteLine($"║ " + Language.GetString("View_SaveSource") + $" : {save.sourceDirectory}");
            Console.WriteLine($"║ " + Language.GetString("View_SaveTarget") + $" : {save.targetDirectory}");
            Console.WriteLine($"║ " + Language.GetString("View_SaveType") + $" : {(save.saveStrategy is FullSave ? Language.GetString("View_FullSave") : Language.GetString("View_DifferentialSave"))}");
        }
        Console.WriteLine("╚════════════════════════════════════");
    }
    public static void NoBackupView()
    {
        Console.WriteLine(Language.GetString("View_NoBackups"));
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
        string typeChoice = InputHelper.ReadLineNotNull(Language.GetString("View_SelectBackupType"));
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

}

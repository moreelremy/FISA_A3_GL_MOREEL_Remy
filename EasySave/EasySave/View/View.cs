using System.Resources;
using static Logs;

/// <summary>
/// Manages all user interactions and console views.
/// </summary>
class View
{
    // ─────────────────────────────────────────
    // 🚀 MAIN MENU DISPLAY METHODS
    // ─────────────────────────────────────────

    /// <summary>
    /// Displays the main menu and prompts the user for a selection.
    /// </summary>
    /// <returns>The user's choice as a string.</returns>
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
    /// Prompts the user to select a language.
    /// </summary>
    /// <returns>The selected language code (e.g., "EN", "FR").</returns>
    public static string GetLanguageChoice()
    {
        return InputHelper.ReadLineNotNull(Language.GetString("View_LanguageChoice"));
    }

    // ─────────────────────────────────────────
    // 🔄 BACKUP MANAGEMENT METHODS
    // ─────────────────────────────────────────

    /// <summary>
    /// Displays a success message when a backup is created.
    /// </summary>
    /// <param name="save">The newly created backup.</param>
    public static void SaveAddedMessageView(Save save)
    {
        Console.WriteLine(string.Format(Language.GetString("View_SaveAddedMessage"), save.name));
    }

    /// <summary>
    /// Displays a list of available backups with details.
    /// </summary>
    /// <param name="saves">A list of all existing backups.</param>
    public static void ShowSavesView(List<Save> saves)
    {
        if (saves.Count == 0)
        {
            NoBackupView();
            return;
        }

        Console.WriteLine("╔═══════════════════════════════════╗");
        Console.WriteLine(Language.GetString("View_ListOfBackups"));
        Console.WriteLine("╚═══════════════════════════════════╝");

        for (int i = 0; i < saves.Count; i++)
        {
            Console.WriteLine($"   {Language.GetString("View_NumberSave")} : [{i + 1}]");
            Console.WriteLine($"   " + Language.GetString("View_SaveName") + $" : {saves[i].name}");
            Console.WriteLine($"   " + Language.GetString("View_SaveSource") + $" : {saves[i].sourceDirectory}");
            Console.WriteLine($"   " + Language.GetString("View_SaveTarget") + $" : {saves[i].targetDirectory}");
            Console.WriteLine($"   " + Language.GetString("View_SaveType") + $" : {(saves[i].saveStrategy is FullSave ? Language.GetString("View_FullSave") : Language.GetString("View_DifferentialSave"))}");
            Console.WriteLine("═════════════════════════════════════");
        }
    }

    /// <summary>
    /// Displays a message when no backups are found.
    /// </summary>
    public static void NoBackupView()
    {
        Console.WriteLine(Language.GetString("View_NoBackups"));
    }

    // ─────────────────────────────────────────
    // 🔍 BACKUP SELECTION & EXECUTION
    // ─────────────────────────────────────────

    /// <summary>
    /// Displays a list of backups available for execution.
    /// </summary>
    /// <param name="saves">List of available backups.</param>
    public static void DisplaySavesForExecution(List<Save> saves)
    {
        Console.WriteLine(Language.GetString("View_ChooseSaveToExecute"));
        for (int i = 0; i < saves.Count; i++)
        {
            Console.WriteLine($"[{i + 1}] {saves[i].name}");
        }
    }

    /// <summary>
    /// Prompts the user to select a save for execution.
    /// </summary>
    /// <param name="maxIndex">The maximum valid index.</param>
    /// <returns>The zero-based index of the selected backup, or -1 if invalid.</returns>
    public static int GetSaveIndexForExecution(int maxIndex)
    {
        string input = InputHelper.ReadLineNotNull(Language.GetString("View_EnterSaveNumber"));
        if (int.TryParse(input, out int index) && index > 0 && index <= maxIndex)
        {
            return index - 1;
        }

        DisplayError(Language.GetString("View_InvalidSelection"));
        return -1;
    }

    // ─────────────────────────────────────────
    // ❌ BACKUP DELETION METHODS
    // ─────────────────────────────────────────

    /// <summary>
    /// Displays the choice between returning to the menu or deleting a save.
    /// </summary>
    /// <returns>The user's choice.</returns>
    public static string ShowChoiceMenuOrDelete()
    {
        return InputHelper.ReadLineNotNull(
            Language.GetString("Controller_ChoiceDeleteOrMenu") + "\n\n" +
            "[1] : " + Language.GetString("Controller_ChoiceMenu") + "\n" +
            "[2] : " + Language.GetString("Controller_ChoiceDelete")
        );
    }

    /// <summary>
    /// Displays a list of saves available for deletion.
    /// </summary>
    /// <param name="saves">List of saves.</param>
    public static void DisplaySavesForDeletion(List<Save> saves)
    {
        Console.WriteLine(Language.GetString("View_ChooseSaveToDelete"));
        for (int i = 0; i < saves.Count; i++)
        {
            Console.WriteLine($"[{i + 1}] {saves[i].name}");
        }
    }

    // ─────────────────────────────────────────
    // 🟢 SUCCESS / ERROR MESSAGES
    // ─────────────────────────────────────────

    /// <summary>
    /// Displays a success message in green.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public static void DisplaySuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    /// <summary>
    /// Displays an error message in red.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public static void DisplayError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    // ─────────────────────────────────────────
    // 🏁 FINALIZATION METHODS
    // ─────────────────────────────────────────

    /// <summary>
    /// Prompts the user to press any key to continue.
    /// </summary>
    public static void PromptToContinue()
    {
        Console.WriteLine(Language.GetString("Controller_PressAnyKey"));
        Console.ReadLine();
    }
    /// <summary>
    /// Ask the user to choose a date for listing logs
    /// </summary>
    /// <returns>The date dd-mm-yyyy</returns>
    public static string GetWantedDate()
    {
        Console.WriteLine(Language.GetString("View_DateChoice"));
        string? result = Console.ReadLine();

        return result == "" ? $"{DateTime.Now:dd-MM-yyyy}" : result;
    }

    /// <summary>
    /// Outputs a message to the console.
    /// </summary>
    /// <param name="output">The message to display.</param>
    public static void Output(string output)
    {
        Console.WriteLine(output);
    }


    /// <summary>
    /// Affiche les informations d'un log dans la console.
    /// </summary>
    /// <param name="log">L'entrée de log à afficher.</param>
    public static void DisplayLog(LogEntry log)
    {
        if (log == null)
        {
            Console.WriteLine("LogEntry is null.");
            return;
        }

        Console.WriteLine($"Timestamp: {log.timestamp}");
        Console.WriteLine($"SaveName: {log.saveName}");
        Console.WriteLine($"Source: {log.source}");
        Console.WriteLine($"Target: {log.target}");
        Console.WriteLine($"Size: {log.size}");
        Console.WriteLine($"TransferTimeMs: {log.transferTimeMs}");
        Console.WriteLine("══════════════════════════════");
    }

}

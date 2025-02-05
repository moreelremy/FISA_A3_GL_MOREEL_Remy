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
        Console.WriteLine($"    [1]: {Language.GetString("CreateBackup")}");
        Console.WriteLine($"    [2]: {Language.GetString("StartBackup")}");
        Console.WriteLine($"    [3]: {Language.GetString("ViewLogs")}");
        Console.WriteLine($"    [4]: {Language.GetString("ChangeLanguage")}");
        Console.WriteLine($"    [5]: {Language.GetString("ViewAllSaves")}");
        Console.WriteLine($"    [6]: {Language.GetString("ExitApp")}\n\n");

        return InputHelper.ReadLineNotNull(Language.GetString("EnterNumber"), allowReturnToMenu: false);
    }

    /// <summary>
    /// Ask the user to choose a language
    /// </summary>
    /// <returns>The selected language code (EN / FR / ..)</returns>
    public static string GetLanguageChoice()
    {
        return InputHelper.ReadLineNotNull(Language.GetString("LanguageChoice"));
    }

    public static void SaveAddedMessageView(Save save)
    {
        Console.WriteLine($"Backup '{save.name}' has been successfully added.");
    }

    public static void AfficherSavesView(List<Save> saves)
    {
        

        Console.WriteLine(Language.GetString("ListOfBackups"));

        foreach (var save in saves)
        {
            Console.WriteLine("────────────────────────────────────────");
            Console.WriteLine($"Name           : {save.name}");
            Console.WriteLine($"Source         : {save.sourceRepository}");
            Console.WriteLine($"Target         : {save.targetRepository}");
            Console.WriteLine($"Save Type      : {(save.saveType is FullSave ? "Full Save" : "Differential Save")}");
            Console.WriteLine($"Date Created   : {save.dateSauvegarde}");
        }

        Console.WriteLine("────────────────────────────────────────");
    }

    public static void NoBackupView()
    {
        Console.WriteLine(Language.GetString("NoBackups"));
    }


    /// <summary>
    /// Collects information from the user to create a new backup, With a return to menu option
    /// </summary>
    /// <returns>The newly created Save object</returns>
    public static Save CreateBackupView()
    {
        string name = InputHelper.ReadLineNotNull(Language.GetString("EnterBackupName"));

        string source = InputHelper.ReadLineNotNull(Language.GetString("EnterSourcePath"));

        string target = InputHelper.ReadLineNotNull(Language.GetString("EnterTargetPath"));

        Console.WriteLine("[1] Full Save");
        Console.WriteLine("[2] Differential Save\n");
        string typeChoice = InputHelper.ReadLineNotNull(Language.GetString("SelectBackupType"));

        ISaveStrategy saveType = typeChoice == "2" ? new DifferentialSave() : new FullSave();

        return new Save
        {
            name = name,
            sourceRepository = source,
            targetRepository = target,
            saveType = saveType,
        };

    }
}

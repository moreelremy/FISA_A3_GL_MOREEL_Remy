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
        Console.WriteLine($"    [5]: {Language.GetString("ExitApp")}\n\n");

        return InputHelper.ReadLineNotNull(Language.GetString("EnterNumber"));
    }

    /// <summary>
    /// Ask the user to choose a language
    /// </summary>
    /// <returns>The selected language code (EN / FR / ..)</returns>
    public static string GetLanguageChoice()
    {
        return InputHelper.ReadLineNotNull(Language.GetString("LanguageChoice"));
    }

    public static string CreateBackupView()
    {
        return InputHelper.ReadLineNotNull(Language.GetString(""));
    }
}

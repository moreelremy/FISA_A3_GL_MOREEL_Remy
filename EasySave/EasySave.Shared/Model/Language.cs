using System.Globalization;
using System.Resources;

/// <summary>
/// Allows you to change the interface language
/// </summary>
public class Language
{
    private static ResourceManager rm = new ResourceManager("EasySave.Shared.Model.Languages.language", typeof(Language).Assembly);
    private static CultureInfo cultureInfo = new CultureInfo("fr");

    /// <summary>
    /// Changes the interface language based on the entered character string
    /// </summary>
    /// <param name="languageCode">Code that defines the interface language</param>
    public static void SetLanguage(string languageCode)
    {
        languageCode = languageCode.ToUpper();
        switch (languageCode)
        {
            case "EN":
                cultureInfo = new CultureInfo("en");
                break;

            case "RU":
                cultureInfo = new CultureInfo("ru");
                if (IsConsoleApplication())
                {
                    Console.OutputEncoding = System.Text.Encoding.UTF8;
                }
                break;

            default:
                cultureInfo = new CultureInfo("fr");
                break;
        }

        Thread.CurrentThread.CurrentCulture = cultureInfo;
    }

    /// <summary>
    /// Allows you to retrieve the text based on the entered key and a variable if you specify it
    /// </summary>
    /// <param name="key">Key linked to a character string in resources</param>
    /// <returns>The character string linked to the key</returns>
    public static string GetString(string key)
    {
        return rm.GetString(key, cultureInfo) ?? key;
    }

    /// <summary>
    /// Method which allows you to know whether to activate UTF 8 or not
    /// </summary>
    /// <returns> Returns true if this is the console application that is launched </returns>
    private static bool IsConsoleApplication()
    {
        return Environment.UserInteractive && !Console.IsOutputRedirected && !Console.IsErrorRedirected;
    }


}
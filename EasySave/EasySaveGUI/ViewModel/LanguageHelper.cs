using System.Globalization;
using System.Resources;


namespace EasySaveGUI.ViewModel
{
    /// <summary>
    /// Class allowing to manage the language of the application
    /// </summary>
    public class LanguageHelper : BaseViewModel 
    {
        private static readonly ResourceManager rm = new ResourceManager("EasySave.Shared.Model.Languages.language", typeof(Language).Assembly);
        private static CultureInfo cultureInfo = new CultureInfo("fr");

        private static LanguageHelper _instance;
        public static LanguageHelper Instance => _instance ??= new LanguageHelper();

        /// <summary>
        /// Allows you to retrieve the text based on the entered key and a variable if you specify it
        /// </summary>
        /// <param name="key"> Key linked to a character string in resources </param>
        /// <returns> The character string linked to the key </returns>
        public string this[string key] => rm.GetString(key, cultureInfo) ?? key;

        /// <summary>
        /// Changes the language of the application
        /// </summary>
        /// <param name="languageCode"> Language code </param>
        public static void ChangeLanguage(string languageCode)
        {
            Language.SetLanguage(languageCode);
            cultureInfo = new CultureInfo(languageCode);
            Instance.OnPropertyChanged(null); 
        }

        /// <summary>
        /// Translates the key into the corresponding string
        /// </summary>
        /// <param name="key"> Key to translate </param>
        /// <returns> Translated string </returns>
        public static string Translate(string key)
        {
            return Instance[key]; // Keeps existing functionality
        }
    }
}

using System.ComponentModel;

namespace EasySaveGUI.ViewModel
{
    /// <summary>
    /// MVVM Helper to get the translation of a key.
    /// </summary>
    public class LanguageHelper : BaseViewModel
    {
        private static LanguageHelper _instance;
        public static LanguageHelper Instance => _instance ??= new LanguageHelper();

        /// <summary>
        /// Key indexer to get the translation of a key.
        /// </summary>
        public string this[string key] => Language.GetString(key);

        /// <summary>
        /// Change the language of the application.
        /// </summary>
        /// <param name="languageCode"> The language code to change to. </param>
        public static void ChangeLanguage(string languageCode)
        {
            Language.SetLanguage(languageCode);
            Instance.OnPropertyChanged(null);
        }
    }
}

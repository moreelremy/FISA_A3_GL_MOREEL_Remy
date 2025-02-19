using System.Globalization;
using System.Resources;


namespace EasySaveGUI.ViewModel
{
    public class LanguageHelper : BaseViewModel 
    {
        private static readonly ResourceManager rm = new ResourceManager("EasySave.Shared.Model.Languages.language", typeof(Language).Assembly);
        private static CultureInfo cultureInfo = new CultureInfo("fr");

        private static LanguageHelper _instance;
        public static LanguageHelper Instance => _instance ??= new LanguageHelper();

        public string this[string key] => rm.GetString(key, cultureInfo) ?? key;

        public static void ChangeLanguage(string languageCode)
        {
            Language.SetLanguage(languageCode);
            cultureInfo = new CultureInfo(languageCode);
            Instance.OnPropertyChanged(null); 
        }

        public static string Translate(string key)
        {
            return Instance[key]; // Keeps existing functionality
        }
    }
}

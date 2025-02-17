using EasySaveGUI.Helpers;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace EasySaveGUI.Helpers
{
    public class LanguageHelper : INotifyPropertyChanged
    {
        private static readonly ResourceManager rm = new ResourceManager("EasySave.Shared.Model.Languages.language", typeof(Language).Assembly);
        private static CultureInfo cultureInfo = new CultureInfo("fr");

        private static LanguageHelper _instance;

        public static LanguageHelper Instance => _instance ??= new LanguageHelper();

        public event PropertyChangedEventHandler? PropertyChanged;


        public string this[string key] => rm.GetString(key, cultureInfo) ?? key;

        public static void ChangeLanguage(string languageCode)
        {
            Language.SetLanguage(languageCode);
            cultureInfo = new CultureInfo(languageCode);
            Instance.NotifyLanguageChanged();
        }

        private void NotifyLanguageChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));

        }
    }
}

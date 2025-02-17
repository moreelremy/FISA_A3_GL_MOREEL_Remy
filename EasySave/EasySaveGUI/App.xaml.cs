using System.Configuration;
using System.Data;
using System.Windows;
using EasySaveGUI.Helpers;

namespace EasySaveGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            LanguageHelper.ChangeLanguage("fr"); // Langue par défaut
        }

    }

}

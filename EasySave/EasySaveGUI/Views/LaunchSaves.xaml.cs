
using EasySaveGUI.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace EasySaveGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LaunchSaves : Window
    {
        public LaunchSaves()
        {
            InitializeComponent();
            DataContext = LanguageHelper.Instance;

        }
    }
}
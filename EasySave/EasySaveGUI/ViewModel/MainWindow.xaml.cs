using System.Configuration;
using EasySaveGUI.Helpers;
using System.Windows;
using System.Windows.Controls;


namespace EasySaveGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = LanguageHelper.Instance;
        }

        private void ButtonMenuCreateSaveClick(object sender, RoutedEventArgs e)
        {
            CreateSave createSaveWindow = new CreateSave();
            createSaveWindow.Show();
            this.Close(); // Close MainWindow
        }

        private void ButtonMenuShowSavesClick(object sender, RoutedEventArgs e)
        {
            ShowSaves showSavesWindow = new ShowSaves();
            showSavesWindow.Show();
            this.Close(); // Close MainWindow
        }

        private void ButtonMenuShowLogsClick(object sender, RoutedEventArgs e)
        {
            ShowLogs showLogsWindow = new ShowLogs();
            showLogsWindow.Show();
            this.Close(); // Close MainWindow
        }

        private void ButtonMenuLaunchSavesClick(object sender, RoutedEventArgs e)
        {
            LaunchSaves launchSavesWindow = new LaunchSaves();
            launchSavesWindow.Show();
            this.Close(); // Close MainWindow
        }
    }
}
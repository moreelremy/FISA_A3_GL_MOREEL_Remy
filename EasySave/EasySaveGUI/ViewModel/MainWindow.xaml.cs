
using System.Windows;


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
        }

        private void Button_CreateSave_Click(object sender, RoutedEventArgs e)
        {
            CreateSave createSaveWindow = new CreateSave();
            createSaveWindow.Show();
            this.Close(); // Close MainWindow
        }

        private void Button_ShowSaves_Click(object sender, RoutedEventArgs e)
        {
            ShowSaves showSavesWindow = new ShowSaves();
            showSavesWindow.Show();
            this.Close(); // Close MainWindow
        }

        private void Button_ShowLogs_Click(object sender, RoutedEventArgs e)
        {
            ShowLogs showLogsWindow = new ShowLogs();
            showLogsWindow.Show();
            this.Close(); // Close MainWindow
        }

        private void Button_LaunchSaves_Click(object sender, RoutedEventArgs e)
        {
            LaunchSaves launchSavesWindow = new LaunchSaves();
            launchSavesWindow.Show();
            this.Close(); // Close MainWindow
        }

        private void Button_Leave_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
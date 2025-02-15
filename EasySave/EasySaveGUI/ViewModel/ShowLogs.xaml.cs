
using System.Windows;

namespace EasySaveGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShowLogs : Window
    {
        public ShowLogs()
        {
            InitializeComponent();
        }

        private void ButtonLogsMenuClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close(); // Close CreateSave window
        }

        private void ButtonLogsLeaveClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
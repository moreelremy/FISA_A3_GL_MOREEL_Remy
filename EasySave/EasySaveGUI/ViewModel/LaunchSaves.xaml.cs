
using System.Windows;

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
        }

        private void Button_Menu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close(); // Close CreateSave window
        }

        private void Button_Leave_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
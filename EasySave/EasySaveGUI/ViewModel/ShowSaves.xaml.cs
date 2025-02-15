
using System.Windows;

namespace EasySaveGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShowSaves : Window
    {
        public ShowSaves()
        {
            InitializeComponent();
        }

        private void ButtonShowSavesMenuClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close(); // Close CreateSave window
        }

        private void ButtonShowSavesLeaveClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
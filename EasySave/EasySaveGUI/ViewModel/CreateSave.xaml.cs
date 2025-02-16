
using EasySaveGUI.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace EasySaveGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CreateSave : Window
    {
        public CreateSave()
        {
            InitializeComponent();
            DataContext = LanguageHelper.Instance;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedLanguage = selectedItem.Tag.ToString();
                LanguageHelper.ChangeLanguage(selectedLanguage);
            }
        }

        private void ButtonCreateSaveMenuClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close(); // Close CreateSave window
        }

        private void ButtonCreateSaveLeaveClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
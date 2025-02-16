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

            // Charger la langue depuis les paramètres
            string savedLanguage = Properties.Settings.Default.Language;

            if (!string.IsNullOrEmpty(savedLanguage))
            {
                LanguageHelper.ChangeLanguage(savedLanguage);
            }

            // Sélectionner la bonne langue dans le ComboBox
            var comboBox = FindName("LanguageComboBox") as ComboBox;
            if (comboBox != null)
            {
                foreach (ComboBoxItem item in comboBox.Items)
                {
                    if (item.Tag != null && item.Tag.ToString() == savedLanguage)
                    {
                        item.IsSelected = true;
                        break;
                    }
                }
            }
        }




        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedLanguage = selectedItem.Tag.ToString();

                // Sauvegarde dans les settings
                Properties.Settings.Default.Language = selectedLanguage;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();

                // Change la langue immédiatement
                LanguageHelper.ChangeLanguage(selectedLanguage);
            }
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

        private void ButtonMenuLeaveClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
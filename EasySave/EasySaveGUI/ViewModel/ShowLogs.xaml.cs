
using EasySaveGUI.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using static Logs;

namespace EasySaveGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShowLogs : Window
    {
        public ObservableCollection<LogEntry> Logs { get; set; }
        public ShowLogs()
        {
            InitializeComponent();

            DataContext = LanguageHelper.Instance;

            LoadSavedLanguage();

            this.DataContext = this; // Sets the current class as the data source

            LoadLogs(GetCurrentFileDate());
        }

        private void LoadLogs(string filePath)
        {
            var logsFromFile = ReadGeneralLog(filePath);
            Debug.WriteLine(Path.GetFullPath(filePath));
            Debug.WriteLine(logsFromFile);

            Logs = new ObservableCollection<LogEntry>(logsFromFile);
            DataGridLogs.ItemsSource = Logs;
        }


        private string GetCurrentFileDate()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs/Logs", $"{DateTime.Now:dd-MM-yyyy}.json");
        }

        private void ButtonLogsSearchClick(object sender, RoutedEventArgs e)
        {
            string wantedDate = InputLogsSearch.Text;
            wantedDate = wantedDate == "" ? $"{DateTime.Now:dd-MM-yyyy}" : wantedDate;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs/Logs", $"{wantedDate}.json");

            if (!File.Exists(filePath))
            {
                MessageBox.Show(LanguageHelper.Translate("WPF_FileNotFound") + $"{wantedDate}");
                return;
            }
            LoadLogs(filePath);
        }

        private void LoadSavedLanguage()
        {
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
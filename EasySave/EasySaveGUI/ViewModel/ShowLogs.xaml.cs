
using EasySaveGUI.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using static Logs;
using System.Text.RegularExpressions;

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

            if (!Regex.IsMatch(wantedDate, "^(0[1-9]|[12][0-9]|3[01])-(0[1-9]|1[0-2])-\\d{4}$"))
            {
                MessageBox.Show(LanguageHelper.Translate("WPF_FormatLog") + " : 15-02-2024, " +LanguageHelper.Translate("WPF_Format")+ " : jj-MM-aaaa).");
                return;
            }

            if (!File.Exists(filePath))
            {
                MessageBox.Show(LanguageHelper.Translate("WPF_FileNotFound") + $"{wantedDate}");
                return;
            }
            LoadLogs(filePath);
        }
    }
}
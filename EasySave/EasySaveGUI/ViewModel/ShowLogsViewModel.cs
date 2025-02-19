using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using static Logs;

namespace EasySaveGUI.ViewModel
{
    /// <summary>
    /// ViewModel responsible for handling log display in the EasySave application.
    /// </summary>
    public class ShowLogsViewModel : BaseViewModel
    {
        /// <summary>
        /// Collection of log entries displayed in the DataGrid.
        /// </summary>
        public ObservableCollection<LogEntry> Logs { get; set; }

        private string _inputWantedDate;

        public ICommand ShowLogsCommand { get; }


        public string InputWantedDate
        {
            get => _inputWantedDate;
            set { _inputWantedDate = value; OnPropertyChanged(); }
        }
        public ShowLogsViewModel()
        {
            Logs = new ObservableCollection<LogEntry>();
            ShowLogsCommand = new RelayCommand(_ => LogsSearch());
        }

        /// <summary>
        /// Executes the log search based on the provided or default date.
        /// </summary>
        private void LogsSearch()
        {
            string wantedDate = InputWantedDate == "" ? $"{DateTime.Now:dd-MM-yyyy}" : InputWantedDate;
            wantedDate = "16-02-2025";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../../Logs/Logs", $"{wantedDate}.json");

            if (!Regex.IsMatch(wantedDate, "^(0[1-9]|[12][0-9]|3[01])-(0[1-9]|1[0-2])-\\d{4}$"))
            {
                MessageBox.Show(LanguageHelper.Translate("WPF_FormatLog") + " : 15-02-2024, " + LanguageHelper.Translate("WPF_Format") + " : jj-MM-aaaa).");
                return;
            }

            if (!File.Exists(filePath))
            {
                MessageBox.Show(LanguageHelper.Translate("WPF_FileNotFound") + $"{wantedDate}");
                return;
            }
            LoadLogs(filePath);
        
        }

        /// <summary>
        /// Loads log entries from a specified file and updates the observable collection.
        /// </summary>
        /// <param name="filePath">The path of the log file to be loaded.</param>
        public void LoadLogs(string filePath)
        {
            var logsFromFile = ReadGeneralLog(filePath);

            Logs.Clear();
            foreach (var log in logsFromFile)
            {
                Logs.Add(log);
            }


        }

    }
}

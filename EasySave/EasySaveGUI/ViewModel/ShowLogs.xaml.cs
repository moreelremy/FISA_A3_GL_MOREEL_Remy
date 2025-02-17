
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
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
                MessageBox.Show($"Il n'existe aucun Logs pour cette date: '{wantedDate}'");
                return;
            }
            LoadLogs(filePath);

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
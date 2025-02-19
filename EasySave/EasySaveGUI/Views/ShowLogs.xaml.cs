
using EasySaveGUI.ViewModel;
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
        public ShowLogs()
        {
            InitializeComponent();


            ShowLogsViewModel objViewModel = new ShowLogsViewModel();

            DataContext = objViewModel;


            objViewModel.LogsSearch();

        }
    }
}
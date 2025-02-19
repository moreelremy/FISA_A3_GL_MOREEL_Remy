
using EasySaveGUI.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using static Logs;
using System.Text.RegularExpressions;
using System.Windows.Media;

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

            InitializePlaceholders();

        }

        private void InitializePlaceholders()
        {
            SetPlaceholder(InputLogsSearch, "WPF_EnterDate");
        }

        private void SetPlaceholder(TextBox textBox, string translationKey)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = LanguageHelper.Translate(translationKey);
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == LanguageHelper.Translate("WPF_EnterDate"))
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = LanguageHelper.Translate("WPF_EnterDate");
                textBox.Foreground = Brushes.Gray;
            }
        }

    }
}
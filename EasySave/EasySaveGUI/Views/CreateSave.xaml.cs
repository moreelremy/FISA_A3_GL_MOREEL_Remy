using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using EasySaveGUI.ViewModel;



namespace EasySaveGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CreateSave : Window
    {
        private SaveRepository saveRepository;
       

        public CreateSave()
        {
            InitializeComponent();
            DataContext = new CreateSaveViewModel(App.saveRepository);
        }
       
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && (textBox.Text == LanguageHelper.Translate("WPF_EnterNameSave") ||
                                    textBox.Text == LanguageHelper.Translate("WPF_SelectSource") ||
                                    textBox.Text == LanguageHelper.Translate("WPF_SelectTarget")))
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black; // Change text color to black when typing
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox.Name == "InputCreateSaveSaveName")
                    textBox.Text = LanguageHelper.Translate("WPF_EnterNameSave");
                else if (textBox.Name == "InputCreateSaveOriginPath")
                    textBox.Text = LanguageHelper.Translate("WPF_SelectSource");
                else if (textBox.Name == "InputCreateSaveTargetPath")
                    textBox.Text = LanguageHelper.Translate("WPF_SelectTarget");

                textBox.Foreground = Brushes.Gray; // Reset text color to gray when placeholder is active
            }
        }

       

        // Allow editing of a TextBox only on double-click
        private void TextBox_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.IsReadOnly = false;
                textBox.Foreground = Brushes.Black;
            }
        }

       

    }
}
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

            LoadSavedLanguage();
            
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
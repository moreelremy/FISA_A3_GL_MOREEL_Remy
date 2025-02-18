using System.Configuration;
using EasySaveGUI.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using System.Linq;



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
            SaveRepository saveRepository = new SaveRepository();
            DataContext = LanguageHelper.Instance;

            InitializeText();
        }

        private void InitializeText()
        {
            InputCreateSaveSaveName.Text = LanguageHelper.Translate("WPF_EnterNameSave");
            InputCreateSaveOriginPath.Text = LanguageHelper.Translate("WPF_SelectSource");
            InputCreateSaveTargetPath.Text = LanguageHelper.Translate("WPF_SelectTarget");
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

        // Open Folder Explorer when button is clicked
        private void ButtonCreateSaveOriginPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = LanguageHelper.Translate("WPF_SelectSource"),
                CheckFileExists = false, // Allows selecting a folder
                CheckPathExists = true,
                FileName = LanguageHelper.Translate("WPF_SelectedFile") // To make it open folders
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                InputCreateSaveOriginPath.Text = selectedPath;
                InputCreateSaveOriginPath.IsReadOnly = true; // Make it read-only
            }
        }


        // Open Folder Explorer for Target Path
        private void ButtonCreateSaveTargetPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = LanguageHelper.Translate("WPF_SelectTarget"),
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = LanguageHelper.Translate("WPF_SelectedFile") // Trick to make it open folders
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                InputCreateSaveTargetPath.Text = selectedPath;
                InputCreateSaveTargetPath.IsReadOnly = true; // Make it read-only
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

        private void ButtonCreateSaveCreate_Click(object sender, RoutedEventArgs e)
        {
            // Check if the TextBoxes are not empty
            if (string.IsNullOrWhiteSpace(InputCreateSaveSaveName.Text) ||
                string.IsNullOrWhiteSpace(InputCreateSaveOriginPath.Text) ||
                string.IsNullOrWhiteSpace(InputCreateSaveTargetPath.Text))
            {
                MessageBox.Show(LanguageHelper.Translate("WPF_FieldProblem"), LanguageHelper.Translate("WPF_Error") , MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Ensure _saveRepository is initialized
            if (saveRepository == null)
            {
                saveRepository = new SaveRepository();
            }

            // Check if the Save Name is unique
            if (saveRepository.GetAllSaves().Any(s => s.name == InputCreateSaveSaveName.Text))
            {
                MessageBox.Show(LanguageHelper.Translate("WPF_NameProblem"), LanguageHelper.Translate("WPF_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Check if the Origin Path exists
            if (!Directory.Exists(InputCreateSaveOriginPath.Text))
            {
                MessageBox.Show(LanguageHelper.Translate("WPF_SourceProblem"), LanguageHelper.Translate("WPF_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Check if the Target Path exists
            if (!Directory.Exists(InputCreateSaveTargetPath.Text))
            {
                MessageBox.Show(LanguageHelper.Translate("WPF_TargetProblem"), LanguageHelper.Translate("WPF_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Determine Save Strategy
            ISaveStrategy selectedStrategy = CheckboxCreateSaveComplete.IsChecked == true
                ? new FullSave()   // ✅ Full Save if "Sauvegarde complète" is selected
                : new DifferentialSave(); // ✅ Differential Save if "Sauvegarde différentielle" is selected

            // Create a new Save
            Save save = new Save
            {
                name = InputCreateSaveSaveName.Text,
                sourceDirectory = InputCreateSaveOriginPath.Text,
                targetDirectory = InputCreateSaveTargetPath.Text,
                saveStrategy = selectedStrategy
            };
            // Display Save Object in MessageBox
            MessageBox.Show(
                LanguageHelper.Translate("View_SaveName")+ $" : {save.name}\n" +
                LanguageHelper.Translate("View_SaveSource")+ $" : {save.sourceDirectory}\n" +
                LanguageHelper.Translate("View_SaveTarget")+ $" : {save.targetDirectory}\n" +
                LanguageHelper.Translate("View_SaveType")+ $" : {save.saveStrategy.GetType().Name}",
                LanguageHelper.Translate("WPF_Details"),
                
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            // Add the Save to the list
            saveRepository.AddSave(save);
            // Show a success message
            MessageBox.Show(LanguageHelper.Translate("WPF_CreateSuccess"), LanguageHelper.Translate("WPF_Success"), MessageBoxButton.OK, MessageBoxImage.Information);
            // Close the window
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

    }
}
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


        private void ButtonCreateSaveMenuClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close(); // Close CreateSave window
        }

        private void ButtonCreateSaveLeaveClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && (textBox.Text == "Entrez un nom de sauvegarde" ||
                                    textBox.Text == "Sélectionnez un fichier source" ||
                                    textBox.Text == "Sélectionnez un dossier de destination"))
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
                    textBox.Text = "Entrez un nom de sauvegarde";
                else if (textBox.Name == "InputCreateSaveOriginPath")
                    textBox.Text = "Sélectionnez un fichier source";
                else if (textBox.Name == "InputCreateSaveTargetPath")
                    textBox.Text = "Sélectionnez un dossier de destination";

                textBox.Foreground = Brushes.Gray; // Reset text color to gray when placeholder is active
            }
        }

        // Open Folder Explorer when button is clicked
        private void ButtonCreateSaveOriginPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Sélectionnez un dossier source",
                CheckFileExists = false, // Allows selecting a folder
                CheckPathExists = true,
                FileName = "Dossier Selectionné" // To make it open folders
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
                Title = "Sélectionnez un dossier de destination",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Dossier Selectionné" // Trick to make it open folders
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
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Le nom de sauvegarde existe déjà.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Check if the Origin Path exists
            if (!Directory.Exists(InputCreateSaveOriginPath.Text))
            {
                MessageBox.Show("Le dossier source n'existe pas.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Check if the Target Path exists
            if (!Directory.Exists(InputCreateSaveTargetPath.Text))
            {
                MessageBox.Show("Le dossier de destination n'existe pas.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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
                $"Nom: {save.name}\n" +
                $"Source: {save.sourceDirectory}\n" +
                $"Cible: {save.targetDirectory}\n" +
                $"Type: {save.saveStrategy.GetType().Name}",
                "Détails de la Sauvegarde",
                
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            // Add the Save to the list
            saveRepository.AddSave(save);
            // Show a success message
            MessageBox.Show("La sauvegarde a été créée avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            // Close the window
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

    }
}
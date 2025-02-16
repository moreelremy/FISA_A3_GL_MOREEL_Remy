
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32; 


namespace EasySaveGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CreateSave : Window
    {
        public CreateSave()
        {
            InitializeComponent();
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

    }
}
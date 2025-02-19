using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace CryptoSoftWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ButtonCryptClick(object sender, RoutedEventArgs e)
        {
            if (InputPathFile.Text != "" && InputEncryptionKey.Text != "")
            {
                CryptoSoft.Crypt.Encrypt(InputPathFile.Text, InputEncryptionKey.Text);
                MessageBox.Show("File Successfuly Crypted");
                InputPathFile.Text = "";
                InputEncryptionKey.Text = "";
            }
            else
            {
                MessageBox.Show("Error: Please select a file");
            }
        }

        /// <summary>
        /// Open Folder Explorer when button is clicked
        /// </summary>
        private void ButtonFileClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select a source file",
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "All files (*.*)|*.*",
                FileName = "File Selected" // To make it open folders
            };

            if (dialog.ShowDialog() == true)
            {
                InputPathFile.Text = dialog.FileName;
                InputPathFile.IsReadOnly = true; // Make it read-only
            }
        }

        /// <summary>
        /// Allow editing of a TextBox only on double-click
        /// </summary>
        private void TextBox_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.IsReadOnly = false;
                textBox.Foreground = Brushes.Black;
            }
        }


        private void ButtonLeaveClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
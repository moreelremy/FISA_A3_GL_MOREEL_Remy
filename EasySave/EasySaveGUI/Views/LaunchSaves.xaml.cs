
using EasySaveGUI.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EasySaveGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LaunchSaves : Window
    {
        public LaunchSaves()
        {
            InitializeComponent();
            DataContext = new ExecuteSavesViewModel(App.saveRepository);

            InitializePlaceholders();
        }

        private void InitializePlaceholders()
        {
            SetPlaceholder(InputLaunchSavesNumberSave, "WPF_CodeSave");
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
            if (textBox != null && (textBox.Text == LanguageHelper.Translate("WPF_CodeSave") ||
                                    textBox.Text == LanguageHelper.Translate("WPF_Extension")))
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
                if (textBox.Name == "InputLaunchSavesNumberSave")
                    textBox.Text = LanguageHelper.Translate("WPF_CodeSave");
                else if (textBox.Name == "InputChooseExtension")
                    textBox.Text = LanguageHelper.Translate("WPF_Extension");

                textBox.Foreground = Brushes.Gray;
            }
        }

    }
}
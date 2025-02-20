using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EasySaveGUI.ViewModel;

namespace EasySaveGUI
{
    public partial class SettingsWindow : Window
    {
      
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = new SettingsWindowViewModel();

            InitializePlaceholders();
        }

        private void InitializePlaceholders()
        {
            SetPlaceholder(InputExtension, "WPF_ExtensionsExample");
            SetPlaceholder(InputSetting, "WPF_SoftwareExample");
        }

        private void SetPlaceholder(TextBox textBox, string translationKey)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = LanguageHelper.Instance[translationKey];
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null &&
                (textBox.Text == LanguageHelper.Instance["WPF_ExtensionsExample"] ||
                 textBox.Text == LanguageHelper.Instance["WPF_SoftwareExample"]))
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
                if (textBox.Name == "InputExtension")
                    textBox.Text = LanguageHelper.Instance["WPF_ExtensionsExample"];
                else if (textBox.Name == "InputSetting")
                    textBox.Text = LanguageHelper.Instance["WPF_SoftwareExample"];

                textBox.Foreground = Brushes.Gray;
            }
        }

    }
}

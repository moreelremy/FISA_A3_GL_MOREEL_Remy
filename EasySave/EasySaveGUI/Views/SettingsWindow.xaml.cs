using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace EasySaveGUI
{
    public partial class SettingsWindow : Window
    {
        private readonly string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../settings.json");

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SaveSetting(object sender, RoutedEventArgs e)
        {
            string input = InputSetting.Text;

            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Please enter a valid setting.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                
                var settings = new { UserInput = input };

                string jsonString = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsFilePath, jsonString);

                MessageBox.Show("Setting saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving setting: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

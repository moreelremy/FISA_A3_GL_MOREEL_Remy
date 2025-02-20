using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using static Logs;

namespace EasySaveGUI.ViewModel
{
    /// <summary>
    /// ViewModel responsible for settings business software in the EasySave application.
    /// </summary>
    public class SettingsWindowViewModel : BaseViewModel
    {
        private string _inputSettingsSoftware;
        private readonly string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../settings.json");

        public ICommand SaveSettingCommand { get; }

        public string InputSettingsSoftware
        {
            get => _inputSettingsSoftware;
            set { _inputSettingsSoftware = value; OnPropertyChanged(); }
        }

        public SettingsWindowViewModel()
        {
            SaveSettingCommand = new RelayCommand(_ => ApplySettings());
        }

        private void ApplySettings()
        {
            
            if (string.IsNullOrWhiteSpace(InputSettingsSoftware))
            {
                MessageBox.Show("Please enter a valid setting.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {

                var settings = new { UserInputSettingsSoftware = InputSettingsSoftware };

                string jsonString = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsFilePath, jsonString);

                MessageBox.Show("Setting saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving setting: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }

}
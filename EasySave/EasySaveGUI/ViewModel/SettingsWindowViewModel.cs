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
        private string _inputSettingsExtension;
        private List<string> _extensions;
        private readonly string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../settings.json");

        public ICommand ApplySettingCommand { get; }




        public List<string> Extensions
        {
            get => _extensions;
            set
            {
                _extensions = value;
            }
        }
        public string InputSettingsExtension
        {
            get => _inputSettingsExtension;
            set { _inputSettingsExtension = value; OnPropertyChanged(); }
        }
        public string InputSettingsSoftware
        {
            get => _inputSettingsSoftware;
            set { _inputSettingsSoftware = value; OnPropertyChanged(); }
        }

        public SettingsWindowViewModel()
        {
            ApplySettingCommand = new RelayCommand(_ => ApplySettings());
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
                ChooseExtension();
                var settings = new { 
                    UserInputSettingsSoftware = InputSettingsSoftware
                };

                string jsonString = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsFilePath, jsonString);

                MessageBox.Show("Setting saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving setting: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ChooseExtension()
        {
            string extensionsEntry = InputSettingsExtension;

            if (string.IsNullOrWhiteSpace(extensionsEntry))
            {
                MessageBox.Show("Veuillez entrer des extensions.");
                return;
            }


            Extensions = extensionsEntry.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (var ext in Extensions)
            {
                MessageBox.Show(ext);
            }
        }

    }

}
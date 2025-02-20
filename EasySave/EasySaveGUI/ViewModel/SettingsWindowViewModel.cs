using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;

namespace EasySaveGUI.ViewModel
{
    public class SettingsWindowViewModel : BaseViewModel
    {
        private string _inputSettingsSoftware;
        private string _inputSettingsExtension;
        private List<string> _extensions;
        private readonly string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../settings.json");

        public ICommand ApplySettingCommand { get; }


        /// <summary>
        /// The extensions to save
        /// </summary>
        public List<string> Extensions
        {
            get => _extensions;
            set
            {
                _extensions = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The extension to save
        /// </summary>
        public string InputSettingsExtension
        {
            get => _inputSettingsExtension;
            set
            {
                _inputSettingsExtension = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The software to save
        /// </summary>
        public string InputSettingsSoftware
        {
            get => _inputSettingsSoftware;
            set
            {
                _inputSettingsSoftware = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SettingsWindowViewModel()
        {
            ApplySettingCommand = new RelayCommand(_ => ApplySettings());
            LoadSettingsFromJson();
        }

        /// <summary>
        /// Load the settings from the settings.json file
        /// </summary>
        private void LoadSettingsFromJson()
        {
            try
            {
                if (File.Exists(settingsFilePath))
                {
                    string settingsJson = File.ReadAllText(settingsFilePath);
                    var settings = JsonSerializer.Deserialize<Dictionary<string, object>>(settingsJson);

                    if (settings != null)
                    {
                        if (settings.TryGetValue("UserInputSettingsSoftware", out var softwareValue))
                        {
                            InputSettingsSoftware = softwareValue.ToString().Replace(" ", "");
                        }

                        if (settings.TryGetValue("ExtensionSelected", out var extensionsValue) && extensionsValue is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
                        {
                            Extensions = jsonElement.EnumerateArray().Select(e => e.GetString().Replace(" ", "")).ToList();
                            InputSettingsExtension = string.Join(";", Extensions);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Save the settings in the settings.json file
        /// </summary>
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
                var settings = new
                {
                    UserInputSettingsSoftware = InputSettingsSoftware,
                    ExtensionSelected = Extensions
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

        /// <summary>
        /// Choose the extension to save
        /// </summary>
        private void ChooseExtension()
        {
            string extensionsEntry = InputSettingsExtension;

            if (string.IsNullOrWhiteSpace(extensionsEntry))
            {
                MessageBox.Show("Veuillez entrer des extensions.");
                return;
            }

            Extensions = extensionsEntry.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(e => e.Replace(" ", "")).ToList();
        }
    }
}

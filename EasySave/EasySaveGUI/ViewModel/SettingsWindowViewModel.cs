using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using EasySaveConsole;

namespace EasySaveGUI.ViewModel
{
    public class SettingsWindowViewModel : BaseViewModel
    {
        private string _inputSettingsSoftware;
        private string _inputSettingsExtensionToCrypt;
        private string _inputSettingsExtensionToPrioritize;
        private string _inputSaturationLimit;
        private List<string> _extensionsToCrypt;
        private List<string> _extensionsToPrioritize;
        private readonly string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../settings.json");

        public ICommand ApplySettingCommand { get; }

        public List<string> ExtensionsToCrypt
        {
            get => _extensionsToCrypt ?? new List<string>();
            set
            {
                _extensionsToCrypt = value;
                OnPropertyChanged();
            }
        }

        public List<string> ExtensionsToPrioritize
        {
            get => _extensionsToPrioritize ?? new List<string>();
            set
            {
                _extensionsToPrioritize = value;
                OnPropertyChanged();
            }
        }

        public string InputSettingsExtensionToCrypt
        {
            get => _inputSettingsExtensionToCrypt ?? string.Empty;
            set
            {
                _inputSettingsExtensionToCrypt = value;
                OnPropertyChanged();
            }
        }

        public string InputSettingsSoftware
        {
            get => _inputSettingsSoftware ?? string.Empty;
            set
            {
                _inputSettingsSoftware = value;
                OnPropertyChanged();
            }
        }

        public string InputSettingsExtensionToPrioritize
        {
            get => _inputSettingsExtensionToPrioritize ?? string.Empty;
            set
            {
                _inputSettingsExtensionToPrioritize = value;
                OnPropertyChanged();
            }
        }

        public string InputSaturationLimit
        {
            get => _inputSaturationLimit ?? string.Empty;
            set
            {
                _inputSaturationLimit = value;
                OnPropertyChanged();
            }
        }

        public SettingsWindowViewModel()
        {
            ApplySettingCommand = new RelayCommand(_ => ApplySettings());
            LoadSettingsFromJson();
        }

        private void LoadSettingsFromJson()
        {
            try
            {
                if (File.Exists(settingsFilePath))
                {
                    var settings = Data.LoadFromJson(settingsFilePath);

                    if (settings != null)
                    {
                        if (settings.TryGetValue("UserInputSettingsSoftware", out var softwareValue))
                        {
                            InputSettingsSoftware = softwareValue?.ToString().Trim() ?? string.Empty;
                        }

                        if (settings.TryGetValue("ExtensionToCrypt", out var extensionsValue) && extensionsValue is JsonElement jsonElementCrypt && jsonElementCrypt.ValueKind == JsonValueKind.Array)
                        {
                            ExtensionsToCrypt = jsonElementCrypt.EnumerateArray().Select(e => e.GetString()?.Replace(" ", "") ?? string.Empty).ToList();
                            InputSettingsExtensionToCrypt = string.Join(";", ExtensionsToCrypt);
                        }

                        if (settings.TryGetValue("ExtensionToPrioritize", out var prioritizeValue) && prioritizeValue is JsonElement jsonElementPrioritize && jsonElementPrioritize.ValueKind == JsonValueKind.Array)
                        {
                            ExtensionsToPrioritize = jsonElementPrioritize.EnumerateArray().Select(e => e.GetString()?.Replace(" ", "") ?? string.Empty).ToList();
                            InputSettingsExtensionToPrioritize = string.Join(";", ExtensionsToPrioritize);
                        }

                        if (settings.TryGetValue("SettingSaturationLimit", out var saturationValue))
                        {
                            InputSaturationLimit = saturationValue?.ToString().Trim() ?? string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                Settings obj = new Settings();
                string extensionsToCrypt = InputSettingsExtensionToCrypt;
                ExtensionsToCrypt = obj.ParseExtensions(extensionsToCrypt);
                string extensionsToPrioritize = InputSettingsExtensionToPrioritize;
                ExtensionsToPrioritize = obj.ParseExtensions(extensionsToPrioritize);
                foreach (var elt in ExtensionsToPrioritize)
                {
                    MessageBox.Show(elt);
                }
                var settings = new
                {
                    UserInputSettingsSoftware = InputSettingsSoftware,
                    ExtensionToCrypt = ExtensionsToCrypt,
                    ExtensionToPrioritize = ExtensionsToPrioritize,
                    SettingSaturationLimit = string.IsNullOrWhiteSpace(InputSaturationLimit) ? null : InputSaturationLimit
                };

                Data.WriteInJson(settings, settingsFilePath);
                MessageBox.Show("Setting saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving setting: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

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
        private string _inputSettingsExtensionToPrioritize;
        private string _inputSaturationLimit;
        private List<string> _extensions;
        private readonly string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../settings.json");

        public ICommand ApplySettingCommand { get; }

        public List<string> Extensions
        {
            get => _extensions ?? new List<string>();
            set
            {
                _extensions = value;
                OnPropertyChanged();
            }
        }

        public string InputSettingsExtension
        {
            get => _inputSettingsExtension ?? string.Empty;
            set
            {
                _inputSettingsExtension = value;
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

                        if (settings.TryGetValue("ExtensionSelected", out var extensionsValue) && extensionsValue is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
                        {
                            Extensions = jsonElement.EnumerateArray().Select(e => e.GetString()?.Replace(" ", "") ?? string.Empty).ToList();
                            InputSettingsExtension = string.Join(";", Extensions);
                        }

                        if (settings.TryGetValue("ExtensionToPrioritize", out var prioritizeValue))
                        {
                            InputSettingsExtensionToPrioritize = prioritizeValue?.ToString().Trim() ?? string.Empty;
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
                ChooseExtension();
                var settings = new
                {
                    UserInputSettingsSoftware = InputSettingsSoftware,
                    ExtensionSelected = Extensions,
                    ExtensionToPrioritize = string.IsNullOrWhiteSpace(InputSettingsExtensionToPrioritize) ? null : InputSettingsExtensionToPrioritize,
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

        private void ChooseExtension()
        {
            string extensionsEntry = InputSettingsExtension;

            if (!string.IsNullOrWhiteSpace(extensionsEntry))
            {
                Extensions = extensionsEntry.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(e => e.Replace(" ", "")).ToList();
            }
        }
    }
}

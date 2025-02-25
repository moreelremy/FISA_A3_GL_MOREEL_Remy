using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using SettingsTest;

namespace EasySaveGUI.ViewModel
{
    public class SettingsWindowViewModel : BaseViewModel
    {
        private string _inputSettingsSoftware;
        private string _inputSettingsExtensionsToCrypt;
        private string _inputSettingsExtensionsToPrioritize;
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

        public string InputSettingsExtensionsToCrypt
        {
            get => _inputSettingsExtensionsToCrypt ?? string.Empty;
            set
            {
                _inputSettingsExtensionsToCrypt = value;
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

        public string InputSettingsExtensionsToPrioritize
        {
            get => _inputSettingsExtensionsToPrioritize ?? string.Empty;
            set
            {
                _inputSettingsExtensionsToPrioritize = value;
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
                SettingsGUI obj = new SettingsGUI();
                obj.LoadSettings();
                InputSettingsSoftware = obj.UserInputSettingsSoftware;
                InputSettingsExtensionsToCrypt = string.Join(";", obj.ExtensionsToCrypt);
                InputSettingsExtensionsToPrioritize = string.Join(";", obj.ExtensionsToPrioritize);
                InputSaturationLimit = obj.SettingSaturationLimit;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplySettings()
        {
            SettingsGUI obj = new SettingsGUI(
                InputSettingsSoftware,
                InputSettingsExtensionsToCrypt, 
                InputSettingsExtensionsToPrioritize, 
                InputSaturationLimit
                );
            try
            {
                obj.SaveSettings();
                MessageBox.Show("Successfuly saving setting:", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Error saving setting: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
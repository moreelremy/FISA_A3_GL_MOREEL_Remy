using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using SettingsModel;

namespace EasySaveGUI.ViewModel
{
    public class SettingsWindowViewModel : BaseViewModel
    {
        private string _inputSettingsSoftware;
        private string _inputSettingsExtensionsToCrypt;
        private string _inputSettingsExtensionsToPrioritize;
        private string _inputSaturationLimit;

        public ICommand ApplySettingCommand { get; }


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
                InputSettingsSoftware = obj.SettingsSoftware;
                InputSettingsExtensionsToCrypt = string.Join(";", obj.ExtensionsToCrypt);
                InputSettingsExtensionsToPrioritize = string.Join(";", obj.ExtensionsToPrioritize);
                InputSaturationLimit = obj.SettingSaturationLimit.ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format(Language.GetString("WPF_ErrorLoadSettings"), ex.Message), LanguageHelper.Instance["WPF_Error"], MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplySettings()
        {
            int? saturationLimit = int.TryParse(InputSaturationLimit, out int result) ? result : (int?)null;


            SettingsGUI obj = new SettingsGUI(
                InputSettingsSoftware,
                InputSettingsExtensionsToCrypt, 
                InputSettingsExtensionsToPrioritize,
                saturationLimit
                );


            try
            {
                obj.SaveSettings(); 
                MessageBox.Show(LanguageHelper.Instance["WPF_SettingsSaved"], LanguageHelper.Instance["WPF_Success"], MessageBoxButton.OK, MessageBoxImage.Information);

            }

            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Language.GetString("WPF_ErrorSettings"), ex.Message), LanguageHelper.Instance["WPF_Error"], MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
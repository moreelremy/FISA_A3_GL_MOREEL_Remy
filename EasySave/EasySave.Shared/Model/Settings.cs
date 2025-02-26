using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SettingsModel
{
    public abstract class Settings
    {
        protected static string settingsFilePath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../settings.json");
        public string? UserInputSettingsSoftware { get; set; }
        public List<string>? ExtensionsToCrypt { get; set; }
        public List<string>? ExtensionsToPrioritize { get; set; }
        public string? SettingSaturationLimit { get; set; }
        public abstract void LoadSettings();
        public abstract void SaveSettings();


        public List<string> ParseExtensions(string extensionsEntry)
        {
            if (!string.IsNullOrWhiteSpace(extensionsEntry))
            {
                return extensionsEntry.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(e => e.Replace(" ", "")).ToList();
            }
            return new List<string>();
        }
    }

    public class SettingsConsole : Settings
    {
        public override void LoadSettings()
        {
            if (File.Exists(settingsFilePath))
            {
                var settingsData = Data.LoadFromJson(settingsFilePath);
                this.UserInputSettingsSoftware = settingsData.ContainsKey("UserInputSettingsSoftware") ? settingsData["UserInputSettingsSoftware"].ToString() : "";
                this.ExtensionsToCrypt = settingsData.ContainsKey("ExtensionsToCrypt") ? JsonSerializer.Deserialize<List<string>>(settingsData["ExtensionsToCrypt"].ToString()) : new List<string>();
                this.ExtensionsToPrioritize = settingsData.ContainsKey("ExtensionsToPrioritize") ? JsonSerializer.Deserialize<List<string>>(settingsData["ExtensionsToPrioritize"].ToString()) : new List<string>();
                this.SettingSaturationLimit = settingsData.ContainsKey("SettingSaturationLimit") ? settingsData["SettingSaturationLimit"].ToString() : "";
            }
        }


        public override void SaveSettings()
        {

            var settingsData = new Dictionary<string, object>
        {
            { "UserInputSettingsSoftware", this.UserInputSettingsSoftware },
            { "ExtensionsToCrypt", this.ExtensionsToCrypt },
            { "ExtensionsToPrioritize", this.ExtensionsToPrioritize },
            { "SettingSaturationLimit", this.SettingSaturationLimit }
        };
            Data.WriteInJson(settingsData, settingsFilePath);
        }


        public List<string> ParseExtensions(string extensionsEntry)
        {
            if (!string.IsNullOrWhiteSpace(extensionsEntry))
            {
                return extensionsEntry.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(e => e.Replace(" ", "")).ToList();
            }
            return new List<string>();
        }
    }


    public class SettingsGUI : Settings
    {
        public SettingsGUI() { }
        public SettingsGUI(
            string inputSoftware,
            string inputExtensionToCrypt, 
            string inputExtensionToPrioritize, 
            string inputSaturationLimit)
        {
            UserInputSettingsSoftware = inputSoftware;
            ExtensionsToCrypt = ParseExtensions(inputExtensionToCrypt);
            ExtensionsToPrioritize = ParseExtensions(inputExtensionToPrioritize);
            SettingSaturationLimit = inputSaturationLimit;
        }

        public override void LoadSettings()
        {
            try
            {
                var settings = Data.LoadFromJson(settingsFilePath);

                if (settings != null)
                {
                    if (settings.TryGetValue("UserInputSettingsSoftware", out var softwareValue))
                    {
                        this.UserInputSettingsSoftware = softwareValue?.ToString().Trim() ?? string.Empty;
                    }

                    if (settings.TryGetValue("ExtensionsToCrypt", out var extensionsValue) && extensionsValue is JsonElement jsonElementCrypt && jsonElementCrypt.ValueKind == JsonValueKind.Array)
                    {
                        this.ExtensionsToCrypt = jsonElementCrypt.EnumerateArray().Select(e => e.GetString()?.Replace(" ", "") ?? string.Empty).ToList();
                    }

                    if (settings.TryGetValue("ExtensionsToPrioritize", out var prioritizeValue) && prioritizeValue is JsonElement jsonElementPrioritize && jsonElementPrioritize.ValueKind == JsonValueKind.Array)
                    {
                        this.ExtensionsToPrioritize = jsonElementPrioritize.EnumerateArray().Select(e => e.GetString()?.Replace(" ", "") ?? string.Empty).ToList();
                    }

                    if (settings.TryGetValue("SettingSaturationLimit", out var saturationValue))
                    {
                        this.SettingSaturationLimit = saturationValue?.ToString().Trim() ?? string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        public override void SaveSettings()
        {
            try
            {
                var settings = new
                {
                    UserInputSettingsSoftware = this.UserInputSettingsSoftware,
                    ExtensionsToCrypt = this.ExtensionsToCrypt,
                    ExtensionsToPrioritize = this.ExtensionsToPrioritize,
                    SettingSaturationLimit = this.SettingSaturationLimit
                };

                Data.WriteInJson(settings, settingsFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
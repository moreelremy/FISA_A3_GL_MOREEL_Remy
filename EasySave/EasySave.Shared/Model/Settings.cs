using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EasySaveConsole
{
    public class Settings
    {
        public string UserInputSettingsSoftware { get; set; }
        public List<string> ExtensionSelected { get; set; }
        public string ExtensionToPrioritize { get; set; }
        public string SettingSaturationLimit { get; set; }

        private static readonly string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../settings.json");

        public static Settings LoadSettings()
        {
            if (File.Exists(settingsFilePath))
            {
                var settingsData = Data.LoadFromJson(settingsFilePath);
                return new Settings
                {
                    UserInputSettingsSoftware = settingsData.ContainsKey("UserInputSettingsSoftware")
                        ? settingsData["UserInputSettingsSoftware"]?.ToString() ?? ""
                        : "",

                    ExtensionSelected = settingsData.ContainsKey("ExtensionSelected")
                        ? JsonSerializer.Deserialize<List<string>>(settingsData["ExtensionSelected"]?.ToString() ?? "[]") ?? new List<string>()
                        : new List<string>(),

                    ExtensionToPrioritize = settingsData.ContainsKey("ExtensionToPrioritize")
                        ? settingsData["ExtensionToPrioritize"]?.ToString() ?? ""
                        : "",

                    SettingSaturationLimit = settingsData.ContainsKey("SettingSaturationLimit")
                        ? settingsData["SettingSaturationLimit"]?.ToString() ?? ""
                        : ""
                };
            }

            return new Settings { ExtensionSelected = new List<string>() };
        }


        public void SaveSettings()
        {
            var settingsData = new Dictionary<string, object>
            {
                { "UserInputSettingsSoftware", UserInputSettingsSoftware },
                { "ExtensionSelected", ExtensionSelected },
                { "ExtensionToPrioritize", ExtensionToPrioritize },
                { "SettingSaturationLimit", SettingSaturationLimit }
            };

            Data.WriteInJson(settingsData, settingsFilePath);
        }
    }
}

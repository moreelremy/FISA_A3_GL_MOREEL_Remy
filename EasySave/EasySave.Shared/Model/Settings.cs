using System.Text.Json;

namespace EasySaveConsole
{
    public class Settings
    {
        public string UserInputSettingsSoftware { get; set; }
        public List<string> ExtensionToCrypt { get; set; }
        public List<string> ExtensionToPrioritize { get; set; }
        public string SettingSaturationLimit { get; set; }

        private static readonly string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../settings.json");

        public static Settings LoadSettings()
        {
            if (File.Exists(settingsFilePath))
            {
                var settingsData = Data.LoadFromJson(settingsFilePath);
                return new Settings
                {
                    UserInputSettingsSoftware = settingsData.ContainsKey("UserInputSettingsSoftware") ? settingsData["UserInputSettingsSoftware"].ToString() : "",
                    ExtensionToCrypt = settingsData.ContainsKey("ExtensionToCrypt") ? JsonSerializer.Deserialize<List<string>>(settingsData["ExtensionToCrypt"].ToString()) : new List<string>(),
                    ExtensionToPrioritize = settingsData.ContainsKey("ExtensionToPrioritize") ? JsonSerializer.Deserialize<List<string>>(settingsData["ExtensionToPrioritize"].ToString()) : new List<string>(),
                    SettingSaturationLimit = settingsData.ContainsKey("SettingSaturationLimit") ? settingsData["SettingSaturationLimit"].ToString() : ""
                };
            }
            return new Settings
            {
                ExtensionToCrypt = new List<string>(),
                ExtensionToPrioritize = new List<string>()
            };
        }


        public void SaveSettings()
        {
            var settingsData = new Dictionary<string, object>
            {
                { "UserInputSettingsSoftware", UserInputSettingsSoftware },
                { "ExtensionToCrypt", ExtensionToCrypt },
                { "ExtensionToPrioritize", ExtensionToPrioritize },
                { "SettingSaturationLimit", SettingSaturationLimit }
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
}

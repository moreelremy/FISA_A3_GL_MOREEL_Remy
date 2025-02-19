using System.Configuration;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Windows;
using EasySaveGUI.ViewModel;

namespace EasySaveGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static SaveRepository saveRepository { get; } = new SaveRepository();

        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            string repositoryStatePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../../RepositoryState.json");
            if (File.Exists(repositoryStatePath))
            {
                using JsonDocument repositoryState = JsonDocument.Parse(File.ReadAllText(repositoryStatePath));
                foreach (JsonElement saveState in repositoryState.RootElement.EnumerateArray())
                {
                    saveRepository.AddSave(
                       new Save
                       {
                           name = saveState.GetProperty("name").GetString(),
                           sourceDirectory = saveState.GetProperty("sourceDirectory").GetString(),
                           targetDirectory = saveState.GetProperty("targetDirectory").GetString(),
                           saveStrategy = saveState.GetProperty("saveStrategy").GetString() == "FullSave" ? new FullSave() : new DifferentialSave()
                       }
                   );
                }
            }
        }
        public App()
        {
            LanguageHelper.ChangeLanguage("fr"); // Langue par défaut
            this.Exit += Application_Exit;
        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
          
            string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "../../../../RepositoryState.json");
            if (File.Exists(pathFile))
            {
                File.Delete(pathFile);
            }
            Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
            List<dynamic> savesSates = new List<dynamic>();
            foreach (Save save in saveRepository.GetAllSaves())
            {
                string saveStrategy = save.saveStrategy.GetType().Name;
                string jsonEntry = JsonSerializer.Serialize(save);
                jsonEntry = jsonEntry.Replace("{}", $"\"{saveStrategy}\"");
                savesSates.Add(JsonSerializer.Deserialize<dynamic>(jsonEntry));
            };
            string repositoryState = JsonSerializer.Serialize(savesSates, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(pathFile, repositoryState);
        }

        
    }

}

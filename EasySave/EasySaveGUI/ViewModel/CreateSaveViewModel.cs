using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Text.Json;


namespace EasySaveGUI.ViewModel
{
    public class CreateSaveViewModel : BaseViewModel
    {
        private readonly SaveRepository _saveRepository;
        public ObservableCollection<Save> Saves => _saveRepository.Saves;

        private string _saveName;
        private string _sourcePath;
        private string _targetPath;
        private string _selectedSaveType;
        private string _selectedLogFileExtension;


        public string SaveName
        {
            get => _saveName;
            set { _saveName = value; OnPropertyChanged(); }
        }

        public string SourcePath
        {
            get => _sourcePath;
            set { _sourcePath = value; OnPropertyChanged(); }
        }

        public string TargetPath
        {
            get => _targetPath;
            set { _targetPath = value; OnPropertyChanged(); }
        }


        public string SelectedSaveType
        {
            get => _selectedSaveType;
            set
            {
                _selectedSaveType = value;
                OnPropertyChanged(nameof(SelectedSaveType));
            }
        }

        public string SelectedLogFileExtension {

            get => _selectedLogFileExtension;
            set
            {
                _selectedLogFileExtension = value;
                OnPropertyChanged(nameof(SelectedLogFileExtension));
            }

        }

        // Boolean properties for binding
        
        public bool IsFullSave
        {
            get => SelectedSaveType == "FullSave";
            set
            {
                if (value) SelectedSaveType = "FullSave";
                OnPropertyChanged(nameof(IsFullSave));
                OnPropertyChanged(nameof(IsDifferentialSave)); // Update both
            }
        }

        public bool IsDifferentialSave
        {
            get => SelectedSaveType == "DifferentialSave";
            set
            {
                if (value) SelectedSaveType = "DifferentialSave";
                OnPropertyChanged(nameof(IsDifferentialSave));
                OnPropertyChanged(nameof(IsFullSave)); // Update both
            }
        }

      
        public bool IsJson
        {
            get => SelectedLogFileExtension == "json";
            set
            {
                if (value) SelectedLogFileExtension = "json";
                OnPropertyChanged(nameof(IsJson));
                OnPropertyChanged(nameof(IsXml)); // Update both
            }
        }

        public bool IsXml
        {
            get => SelectedLogFileExtension == "xml";
            set
            {
                if (value) SelectedLogFileExtension = "xml";
                OnPropertyChanged(nameof(IsXml));
                OnPropertyChanged(nameof(IsJson)); // Update both
            }
        }

        public ICommand CreateSaveCommand { get; }
        public ICommand SelectSourceCommand { get; }
        public ICommand SelectTargetCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand ExitCommand { get; }

        public CreateSaveViewModel()
        {
            // Default constructor
        }
        public CreateSaveViewModel(SaveRepository saveRepository)
        {
            _saveRepository = saveRepository;
            SelectedSaveType = "FullSave";
            SelectedLogFileExtension = "json";



            CreateSaveCommand = new RelayCommand(_ => ExecuteCreateSave());
            SelectSourceCommand = new RelayCommand(_ => ExecuteSelectSource());
            SelectTargetCommand = new RelayCommand(_ => ExecuteSelectTarget());
            GoBackCommand = new RelayCommand(_ => ExecuteGoBack());
            ExitCommand = new RelayCommand(_ => ExecuteExit());
        }

        private void ExecuteCreateSave()
        {
            if (string.IsNullOrWhiteSpace(SaveName) ||
                string.IsNullOrWhiteSpace(SourcePath) ||
                string.IsNullOrWhiteSpace(TargetPath))
            {
                MessageBox.Show(LanguageHelper.Instance["WPF_FieldProblem"],
                    LanguageHelper.Instance["WPF_Error"], MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_saveRepository.GetAllSaves().Any(s => s.name == SaveName))
            {
                MessageBox.Show(LanguageHelper.Instance["WPF_NameProblem"],
                    LanguageHelper.Instance["WPF_Error"], MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Directory.Exists(SourcePath))
            {
                MessageBox.Show(LanguageHelper.Instance["WPF_SourceProblem"],
                    LanguageHelper.Instance["WPF_Error"], MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Directory.Exists(TargetPath))
            {
                MessageBox.Show(LanguageHelper.Instance["WPF_TargetProblem"],
                    LanguageHelper.Instance["WPF_Error"], MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SelectedSaveType == null)
            {
                MessageBox.Show(LanguageHelper.Instance["WPF_TypeProblem"],
                    LanguageHelper.Instance["WPF_Error"], MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(SelectedLogFileExtension == null)
            {
                MessageBox.Show(LanguageHelper.Instance["WPF_ExtensionProblem"],
                    LanguageHelper.Instance["WPF_Error"], MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SaveStrategyFactory FactoryStrategy = new SaveStrategyFactory();
            Save save = new Save
            {
                name = SaveName,
                sourceDirectory = SourcePath,
                targetDirectory = TargetPath,
                saveStrategy = FactoryStrategy.CreateSaveStrategy(SelectedSaveType),
                logFileExtension = SelectedLogFileExtension
            };
            
            _saveRepository.AddSave(save);
            // Reset Inputs
            SaveName = string.Empty;
            SourcePath = string.Empty;
            TargetPath = string.Empty;
            SelectedSaveType = "FullSave";         
            SelectedLogFileExtension = "json";      

            MessageBox.Show(LanguageHelper.Instance["WPF_CreateSuccess"],
                LanguageHelper.Instance["WPF_Success"], MessageBoxButton.OK, MessageBoxImage.Information);

            
        }
        
        private void ExecuteSelectSource()
        {
            var dialog = new OpenFileDialog
            {
                Title = LanguageHelper.Instance["WPF_SelectSource"],
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = LanguageHelper.Instance["WPF_SelectedFile"]
            };

            if (dialog.ShowDialog() == true)
            {
                SourcePath = Path.GetDirectoryName(dialog.FileName);
            }
        }

        private void ExecuteSelectTarget()
        {
            var dialog = new OpenFileDialog
            {
                Title = LanguageHelper.Instance["WPF_SelectTarget"],
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = LanguageHelper.Instance["WPF_SelectedFile"]
            };

            if (dialog.ShowDialog() == true)
            {
                TargetPath = Path.GetDirectoryName(dialog.FileName);
            }
        }

        private void ExecuteGoBack()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Application.Current.Windows.OfType<CreateSave>().FirstOrDefault()?.Close();
        }

        private void ExecuteExit()
        {
            Application.Current.Shutdown();
        }
    }
}

using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace EasySaveGUI.ViewModel
{
    public class ExecuteSavesViewModel : BaseViewModel
    {
        private SaveRepository _saveRepository;
        private Save _selectedSave;
        private string _selectedSaveName;

        public ObservableCollection<Save> Saves { get; set; }

        public ICommand ExecuteGlobalSaveCommand { get; }
        public ICommand ExecutePartialSaveCommand { get; }
        public ICommand ChooseExtensionCommand { get; }

        private List<string> _extensions;
        private string _inputExtensions;

        public List<string> Extensions
        {
            get => _extensions;
            set
            {
                _extensions = value;
            }
        }

        public string InputExtensions
        {
            get => _inputExtensions;
            set { _inputExtensions = value; OnPropertyChanged(); }
        }

        public Save SelectedSave
        {
            get => _selectedSave;
            set
            {
                _selectedSave = value;
                OnPropertyChanged();
                SelectedSaveName = _selectedSave?.name;
            }
        }

        public string SelectedSaveName
        {
            get => _selectedSaveName;
            set
            {
                _selectedSaveName = value;
                OnPropertyChanged();
            }
        }

        public ExecuteSavesViewModel()
        {
            // Default constructor
        }

        public ExecuteSavesViewModel(SaveRepository saveRepository)
        {
            _saveRepository = saveRepository;
            LoadSaves();

            ExecuteGlobalSaveCommand = new RelayCommand(_ => ExecuteGlobalSave());
            ExecutePartialSaveCommand = new RelayCommand(_ => ExecutePartialSave());
            ChooseExtensionCommand = new RelayCommand(_ => ChooseExtension());
        }

        private void LoadSaves()
        {
            Saves = new ObservableCollection<Save>(
                _saveRepository.GetAllSaves().Select((s, index) =>
                {
                    s.DisplayIndex = index + 1;
                    return s;
                })
            );
            OnPropertyChanged(nameof(Saves));
        }

        private void ExecuteGlobalSave()
        {
            if (Saves.Count == 0)
            {
                MessageBox.Show("No saves available to execute.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (var save in Saves.ToList())
            {
                if (_saveRepository.ExecuteSave(save, out string errorMessage))
                {
                    MessageBox.Show($"Save '{save.name}' executed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ChooseExtension()
        {
            string extensionsEntry = InputExtensions;
            if (string.IsNullOrWhiteSpace(extensionsEntry))
            {
                MessageBox.Show("Veuillez entrer des extensions.");
                return;
            }

            Extensions = extensionsEntry.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (var ext in Extensions)
            {
                MessageBox.Show(ext);
            }
        }

        private void ExecutePartialSave()
        {
            List<Save> savesToExecute = new List<Save>();

            if (!string.IsNullOrEmpty(SelectedSaveName))
            {
                // If input is a number and skip number (e.g., "1;3;5")
                if (SelectedSaveName.Contains(";"))
                {
                    var indexes = SelectedSaveName.Split(';');
                    foreach (var indexStr in indexes)
                    {
                        if (int.TryParse(indexStr, out int i))
                        {
                            if (i >= 1 && i <= Saves.Count)
                            {
                                var save = Saves.FirstOrDefault(s => s.DisplayIndex == i);
                                if (save != null && !savesToExecute.Contains(save))
                                    savesToExecute.Add(save);
                            }
                            else
                            {
                                MessageBox.Show($"Invalid index '{i}'. Please enter a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Invalid input '{indexStr}'. Please enter valid numbers.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }
                // If input is a range (e.g., "1-5")
                else if (SelectedSaveName.Contains("-"))
                {
                    var rangeParts = SelectedSaveName.Split('-');

                    if (rangeParts.Length == 2 &&
                        int.TryParse(rangeParts[0], out int start) &&
                        int.TryParse(rangeParts[1], out int end))
                    {
                        if (start >= 1 && end <= Saves.Count && start <= end)
                        {
                            savesToExecute = Saves.Where(s => s.DisplayIndex >= start && s.DisplayIndex <= end).ToList();
                        }
                        else
                        {
                            MessageBox.Show("Invalid range. Please enter a valid range like '1-5'.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }
                // If input is a single number
                else if (int.TryParse(SelectedSaveName, out int index))
                {
                    if (index >= 1 && index <= Saves.Count)
                    {
                        var save = Saves.FirstOrDefault(s => s.DisplayIndex == index);
                        if (save != null)
                            savesToExecute.Add(save);
                    }
                    else
                    {
                        MessageBox.Show($"Invalid index '{index}'. Please enter a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                // If input is a save name
                else
                {
                    Save saveToExecute = Saves.FirstOrDefault(s => s.name.Equals(SelectedSaveName, StringComparison.OrdinalIgnoreCase));
                    if (saveToExecute != null)
                    {
                        savesToExecute.Add(saveToExecute);
                    }
                    else
                    {
                        MessageBox.Show($"Save '{SelectedSaveName}' not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }
            // If selection is made via DataGrid
            else if (SelectedSave != null)
            {
                savesToExecute.Add(SelectedSave);
            }
            else
            {
                MessageBox.Show("Please select or enter a save name/index to execute.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Execute selected saves
            foreach (var save in savesToExecute)
            {
                if (_saveRepository.ExecuteSave(save, out string errorMessage))
                {
                    MessageBox.Show($"Save '{save.name}' executed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

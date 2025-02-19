using System.Collections.ObjectModel;
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
            //Default constructor
        }

        public ExecuteSavesViewModel(SaveRepository saveRepository)
        {
            _saveRepository = saveRepository;
            Saves = new ObservableCollection<Save>(_saveRepository.GetAllSaves());

            ExecuteGlobalSaveCommand = new RelayCommand(_ => ExecuteGlobalSave());
            ExecutePartialSaveCommand = new RelayCommand(_ => ExecutePartialSave());
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

        private void ExecutePartialSave()
        {
            if (SelectedSave == null)
            {
                MessageBox.Show("Please select a save to execute.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_saveRepository.ExecuteSave(SelectedSave, out string errorMessage))
            {
                MessageBox.Show($"Save '{SelectedSave.name}' executed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

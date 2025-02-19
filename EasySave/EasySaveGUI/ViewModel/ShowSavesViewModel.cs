using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace EasySaveGUI.ViewModel
{
    public class ShowSavesViewModel : BaseViewModel
    {
        private readonly SaveRepository _saveRepository;
        private Save _selectedSave;
        private string _selectedSaveName;

        public ObservableCollection<Save> Saves { get; set; }

        public ICommand DeleteSaveCommand { get; }

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
        public string SaveStrategyAsString(Save save)
        {
            return save.saveStrategy.GetType().Name;
        }

        public ShowSavesViewModel()
        {
            // Default constructor
        }
        public ShowSavesViewModel(SaveRepository saveRepository)
        {
            _saveRepository = saveRepository ?? throw new ArgumentNullException(nameof(saveRepository)); // Ensure it's not null

            Saves = new ObservableCollection<Save>(saveRepository.GetAllSaves());

            DeleteSaveCommand = new RelayCommand(DeleteSave);
        }

        private void DeleteSave(object parameter )
        {
            string saveName = parameter as string ?? SelectedSaveName; // Handle both input methods

            if (!string.IsNullOrEmpty(saveName))
            {
                var saveToRemove = _saveRepository.GetAllSaves().Find(s => s.name == saveName);
                if (saveToRemove != null)
                {
                    int index = _saveRepository.GetAllSaves().IndexOf(saveToRemove);
                    if (index != -1 && _saveRepository.RemoveSaveByIndex(index))
                    {
                        Saves.Remove(saveToRemove);

                    }
                    MessageBox.Show("Save deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

    }
}

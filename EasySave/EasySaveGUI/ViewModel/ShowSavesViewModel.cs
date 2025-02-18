using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EasySaveGUI.ViewModel
{
    public class ShowSavesViewModel : BaseViewModel
    {
        private SaveRepository _saveRepository;

        public ObservableCollection<Save> Saves { get; set; }

        public ICommand DeleteSaveCommand { get; }

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
            Saves = new ObservableCollection<Save>(saveRepository.GetAllSaves());

            DeleteSaveCommand = new RelayCommand(DeleteSave);
        }

        private void DeleteSave(object parameter)
        {
            if (parameter is string saveName)
            {
                var saveToRemove = _saveRepository.GetAllSaves().Find(s => s.name == saveName);
                if (saveToRemove != null)
                {
                    _saveRepository.GetAllSaves().Remove(saveToRemove);
                    

                    Saves.Remove(saveToRemove);
                }
            }
        }
    }
}

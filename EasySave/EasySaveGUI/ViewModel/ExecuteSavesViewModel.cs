using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EasySaveGUI.ViewModel
{
    public class ExecuteSavesViewModel : BaseViewModel
    {
        private SaveRepository _saveRepository;

        public ObservableCollection<Save> Saves { get; set; }

        public ICommand ExecuteGlobalSaveCommand { get; }
        public ICommand ExecutePartialSaveCommand { get; }

        public ExecuteSavesViewModel()
        {
            // Default constructor
        }

        public ExecuteSavesViewModel(SaveRepository saveRepository)
        {
            Saves = new ObservableCollection<Save>(saveRepository.GetAllSaves());
            
        }


    }


}

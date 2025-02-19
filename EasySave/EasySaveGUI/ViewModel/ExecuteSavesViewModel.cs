using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace EasySaveGUI.ViewModel
{
    public class ExecuteSavesViewModel : BaseViewModel
    {
        private SaveRepository _saveRepository;

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

        public ExecuteSavesViewModel()
        {
            // Default constructor
        }

        public ExecuteSavesViewModel(SaveRepository saveRepository)
        {
            Saves = new ObservableCollection<Save>(saveRepository.GetAllSaves());
            ChooseExtensionCommand = new RelayCommand(_ => ChooseExtension());


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

    }


}

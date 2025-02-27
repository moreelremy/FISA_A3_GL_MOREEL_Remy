using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EasySaveGUI.ViewModel
{
    public class ExecuteSavesViewModel : BaseViewModel
    {
        private SaveRepository _saveRepository;
        private Save _selectedSave;
        private string _selectedSaveName;
        private CancellationTokenSource _cts;
        private ManualResetEventSlim _pauseEvent;

        public ObservableCollection<Save> Saves { get; set; }
        
public ICommand ExecuteGlobalSaveCommand { get; }
      
        public ICommand ExecutePartialSaveCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand ResumeCommand { get; }
        public ICommand StopCommand { get; }

        // Property for binding the progress bar (0–100)
        private int _globalProgress;
        public int GlobalProgress
        {
            get => _globalProgress;
            set { _globalProgress = value; OnPropertyChanged(); }
        }


        private System.Collections.Generic.List<string> _extensions;
        private string _inputExtensions;

        public System.Collections.Generic.List<string> Extensions
        {
            get => _extensions;
            set { _extensions = value; OnPropertyChanged(); }
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
            set { _selectedSaveName = value; OnPropertyChanged(); }
        }

        public ExecuteSavesViewModel() { }

        public ExecuteSavesViewModel(SaveRepository saveRepository)
        {
            _saveRepository = saveRepository;
            LoadSaves();

            ExecuteGlobalSaveCommand = new RelayCommand(_ =>
    Task.Run(() => ExecuteGlobalSave())
);
            ExecutePartialSaveCommand = new RelayCommand(_ => Task.Run(() => ExecutePartialSave()));

            // Initialize control tokens.
            _cts = new CancellationTokenSource();
            _pauseEvent = new ManualResetEventSlim(true);

            PauseCommand = new RelayCommand(_ => PauseBackup());
            ResumeCommand = new RelayCommand(_ => ResumeBackup());
            StopCommand = new RelayCommand(_ => StopBackup());
        }

        private void PauseBackup()
        {
            // Pauses the backup process (effective after the current file finishes).
            _pauseEvent.Reset();
        }

        private void ResumeBackup()
        {
            // Resumes the backup process.
            _pauseEvent.Set();
        }

        private void StopBackup()
        {
            // Requests immediate cancellation.
            _cts.Cancel();
            GlobalProgress = 0;

            OnPropertyChanged(nameof(GlobalProgress));
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

        private  void ExecuteGlobalSave()
        {
            // Task.Run(() =>
            //{
                if (Saves.Count == 0)
                {
                    MessageBox.Show("No saves available to execute.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                SendToSave(Saves.ToList());
            //});
           
        }


        private async void ExecutePartialSave()
        {
            var savesToExecute = new System.Collections.Generic.List<Save>();

            if (!string.IsNullOrEmpty(SelectedSaveName))
            {
                // If input is a semicolon-separated list (e.g., "1;3;5")
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
                // If input is a single number.
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
                // If input is a save name.
                else
                {
                    var saveToExecute = Saves.FirstOrDefault(s => s.name.Equals(SelectedSaveName, StringComparison.OrdinalIgnoreCase));
                    if (saveToExecute != null)
                        savesToExecute.Add(saveToExecute);
                    else
                    {
                        MessageBox.Show($"Save '{SelectedSaveName}' not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }
            else if (SelectedSave != null)
            {
                savesToExecute.Add(SelectedSave);
            }
            else
            {
                MessageBox.Show("Please select or enter a save name/index to execute.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SendToSave(savesToExecute);
        }


        private async void SendToSave(List<Save> saveToExecute)
        {

            // Reset tokens for a new execution.
            _cts = new CancellationTokenSource();
            _pauseEvent.Set(); // ensure not paused
            var semaphore = new SemaphoreSlim(3);
            var tasks = new List<Task>();

            foreach (var save in saveToExecute)
            {
                var task = ExecuterSaveParallele( semaphore,  save);
                tasks.Add(task);
            }
            int n = 1;
    
            try
            {
                await Task.WhenAll(tasks); // Attend la fin de toutes les sauvegardes
                MessageBox.Show("All saves executed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                GlobalProgress = 0;

            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Backup process was cancelled.", "Stopped", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task ExecuterSaveParallele(SemaphoreSlim semaphore, Save save)
        {
            Save sa = save;
            int n = 1;

                await semaphore.WaitAsync(_cts.Token);
                try
                {
                    
                    _pauseEvent.Wait(); // Attend la reprise si pause active

                    if (!await _saveRepository.ExecuteSave(save, _cts.Token, _pauseEvent, (progress) =>
                    {
                        //save.Progress = progress; // Update individual save progress
                        UpdateGlobalProgress();   // Update overall progress
                    }))
                    {
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during save '{save.name}': {ex.Message}");
                }
                finally
                {
                    semaphore.Release();
                }
            
        }





        // Callback to update progress (bound to the Progress property).
        private void UpdateGlobalProgress()
        {
            if (Saves.Any())
            {
                GlobalProgress = 0;//(int)Saves.Average(s => s.Progress);
            }
        }

    }
}

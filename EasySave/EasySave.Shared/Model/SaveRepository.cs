using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

public class SaveRepository
{
    [JsonInclude]
    public ObservableCollection<Save> Saves { get; private set; } = new ObservableCollection<Save>();








    /// <summary>
    /// Adds a new save and persists it to the JSON file.
    /// </summary>
    public void AddSave(Save save)
    {
        if (Saves.Any(s => s.name == save.name))
        {
            throw new Exception(string.Format(Language.GetString("WPF_FieldProblem")));
        }

        Saves.Add(save);

    }

    /// <summary>
    /// Deletes a save by its index and updates the JSON file.
    /// </summary>
    public bool RemoveSaveByIndex(int index)
    {
        if (index >= 0 && index < Saves.Count)
        {
            Saves.RemoveAt(index);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Retrieves all saves.
    /// </summary>
    public List<Save> GetAllSaves()
    {
        return Saves.ToList();
    }

    /// <summary>
    /// Checks if the repository has no saves.
    /// </summary>
    public bool IsEmpty()
    {
        return !Saves.Any();
    }

    /// <summary>
    /// Executes the selected save operation and updates the JSON file.
    /// </summary>
    public async Task<bool> ExecuteSave(Save save, CancellationToken token, ManualResetEventSlim pauseEvent, Action<int> progressCallback )
    {
      
        try
        {

            if (!Directory.Exists(save.sourceDirectory))
            {
               
               throw new Exception(string.Format(Language.GetString("Controller_DirectoryNotFoundError"), save.name, save.sourceDirectory));
                
            }
            // Use a new Save method that supports cancellation/pause
            await save.saveStrategy.Save(save, token, pauseEvent, progressCallback);

           
          
            return true;
        }
        catch (OperationCanceledException)
        {
           
           throw new Exception(string.Format(Language.GetString("Controller_SaveExecutionErrorProcessCancelled"), save.name));
            
        }
        catch (InvalidOperationException message)
        {
           
           throw new Exception(string.Format(Language.GetString("Controller_SaveExecutionErrorProcessRunning"), save.name, message.Message));
            
        }
        catch (Exception ex)
        {
            
            throw new Exception(string.Format(Language.GetString("Controller_SaveExecutionError"), save.name, ex.Message));
           
        }
    }
}

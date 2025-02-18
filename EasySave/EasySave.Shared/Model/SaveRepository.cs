using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
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
            throw new Exception("A save with this name already exists.");
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
    public bool ExecuteSave(Save save, out string errorMessage)
    {
        try
        {
            if (!Directory.Exists(save.sourceDirectory))
            {
                errorMessage = string.Format(Language.GetString("Controller_DirectoryNotFoundError"), save.name, save.sourceDirectory);
                return false;
            }

            save.saveStrategy.Save(save);
            
            errorMessage = null;
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = string.Format(Language.GetString("Controller_SaveExecutionError"), save.name, ex.Message);
            return false;
        }
    }
}

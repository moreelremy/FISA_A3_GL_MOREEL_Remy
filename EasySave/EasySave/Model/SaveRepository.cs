using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

public class SaveRepository
{
    // List of Saves
    [JsonInclude]
    private List<Save> saves = new List<Save>();

    /// <summary>
    /// Adds a save to the list if the maximum limit is not reached.
    /// </summary>
    /// <param name="save">The save to add.</param>
    /// <returns>The save if added successfully, otherwise null.</returns>
    public void AddSave(Save save)
    {
        // Add the save and return it
        saves.Add(save);
    }

    /// <summary>
    /// Deletes a save by its index in the list.
    /// </summary>
    /// <param name="index">The index of the save to delete.</param>
    /// <returns>True if the save was deleted successfully, otherwise false.</returns>
    public bool RemoveSaveByIndex(int index)
    {
        if (index >= 0 && index < saves.Count)
        {
            saves.RemoveAt(index);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Retrieves a list of all saved Backup.
    /// </summary>
    /// <returns>Backup List</returns>
    public List<Save> GetAllSaves()
    {
        return saves;
    }

    /// <summary>
    /// Checks if the list of backups is empty.
    /// </summary>
    /// <returns>True if no backup, otherwise False</returns>
    public bool IsEmpty()
    {
        return saves.Count == 0;
    }

    /// <summary>
    /// Executes a save operation by delegating to the save strategy.
    /// </summary>
    public bool ExecuteSave(Save save, out string errorMessage)
    {
        try
        {
            // Validate the source directory before starting the save
            if (!Directory.Exists(save.sourceDirectory))
            {
                errorMessage = string.Format(Language.GetString("Controller_DirectoryNotFoundError"), save.name, save.sourceDirectory);
                return false;  // Stop execution if the directory does not exist
            }

            // Execute the save using the strategy
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
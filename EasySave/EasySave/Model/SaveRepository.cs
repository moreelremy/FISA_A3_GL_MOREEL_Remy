using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
public class SaveRepository
{
    // List of Saves
    private List<Save> saves = new List<Save>();

    /// <summary>
    /// Adds a save to the list if the maximum limit is not reached.
    /// </summary>
    /// <param name="save">The save to add.</param>
    /// <returns>The save if added successfully, otherwise null.</returns>
    public Save AddSave(Save save)
    {
        // Check if the maximum number of saves has been reached
        if (saves.Count >= 5)
        {
            return null;  // Indicate that the save was not added
        }

        // Add the save and return it
        saves.Add(save);
        return save;
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
    public bool ExecuteSave(Save save)
    {
        try
        {
            // Mark save as active
            UpdateStateFile(save, isActive: true);

            // Execute the save using the appropriate strategy
            save.saveStrategy.Save(save);

            // Mark save as completed
            UpdateStateFile(save, isActive: false);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(Language.GetString("Controller_SaveExecutionError"), save.name, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Updates the state file to reflect the current save status.
    /// </summary>
    private void UpdateStateFile(Save save, bool isActive)
    {
        string stateFilePath = Path.Combine(Directory.GetCurrentDirectory(), "state.json");
        var stateData = new
        {
            SaveName = save.name,
            Timestamp = DateTime.Now.ToString("o"),
            Status = isActive ? "Active" : "Completed"
        };

        string json = JsonSerializer.Serialize(stateData, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(stateFilePath, json);

    }
}
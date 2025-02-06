using System.Xml.Linq;

public class SaveRepository
{
    // List of Saves
    private List<Save> saves = new List<Save>();

    /// <summary>
    /// Add save and return it
    /// </summary>
    /// <param name="save">the returned save</param>    
    /// <returns>the new backup </returns>
    public Save AddSave(Save save)
    {
        saves.Add(save);
        return save;
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
    /// Deletes a save by its name.
    /// </summary>
    /// <param name="name">The name of the save to delete.</param>
    /// <returns>True if the save was deleted successfully, otherwise false.</returns>
    public bool RemoveSave(string name)
    {
        Save saveToRemove = saves.FirstOrDefault(s => s.name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (saveToRemove != null)
        {
            saves.Remove(saveToRemove);
            return true;  // Successfully removed
        }

        return false;  // Save not found
    }

    /// <summary>
    /// Searches for a save by its name.
    /// </summary>
    /// <param name="name">The name of the save to search for.</param>
    /// <returns>The found save or null if no match is found.</returns>
    public Save SearchSave(string name)
    {
        return saves.FirstOrDefault(s => s.name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public void ShowSave()
    {
        foreach (Save save in saves)
        {
            Console.WriteLine(save.name);
            Console.WriteLine(save.sourceDirectory);
            Console.WriteLine(save.targetDirectory);
            Console.WriteLine(save.saveStrategy);

        }
    }

}
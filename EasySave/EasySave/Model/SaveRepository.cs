using System.Text.Json.Serialization;
using System.Xml.Linq;

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
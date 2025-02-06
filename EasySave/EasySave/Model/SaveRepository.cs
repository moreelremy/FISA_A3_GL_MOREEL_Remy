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

    public void RemoveSave()
    {
        throw new NotImplementedException();
    }

    public void SearchSave()
    {
        throw new NotImplementedException();
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
public class SaveRepository
{
    // Liste interne des sauvegardes
    private List<Save> saves = new List<Save>();

    /// <summary>
    /// Add save and return it
    /// </summary>
    /// <param name="save">the returned save</param>    
    /// <returns>the new backup </returns>
    public Save AjouterSave(Save save)
    {
        saves.Add(save);
        return save;
    }

    /// <summary>
    /// Retrieves a list of all saved Backup.
    /// </summary>
    /// <returns>Backup List</returns>
    public List<Save> ObtenirToutesLesSaves()
    {
        return saves;
    }

    /// <summary>
    /// Checks if the list of backups is empty.
    /// </summary>
    /// <returns>True if no backup, otherwise False</returns>
    public bool EstVide()
    {
        return saves.Count == 0;
    }

    public void SupprimerSave()
    {
        throw new NotImplementedException();

    }

    public void RechercherSave()
    {
        throw new NotImplementedException();

    }

    public void AfficherSave()
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
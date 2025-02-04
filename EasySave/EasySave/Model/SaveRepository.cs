public class SaveRepository
{
    private List<Save> saves = new List<Save>();

    public void AjouterSave()
    {
        var newSave = new Save
        {
            name = "Save1",
            sourceRepository = "C:/Users/Utilisateur/Desktop/Source",
            targetRepository = "C:/Users/Utilisateur/Desktop/Target",
            saveType = new FullSave(),
            dateSauvegarde = DateTime.Now
        };

        saves.Add(newSave);
        Console.WriteLine($"Save '{newSave.name}' added successfully.");


    }

    public void SupprimerSave()
    {

    }

    public void RechercherSave()
    {

    }

    public void AfficherSave()
    {

    }

}
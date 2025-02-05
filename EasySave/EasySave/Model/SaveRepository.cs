public class SaveRepository
{
    private List<Save> saves = new List<Save>();

    public Save AjouterSave(Save save)
    {
        
        saves.Add(save);
        return save;


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
        throw new NotImplementedException();

    }

}
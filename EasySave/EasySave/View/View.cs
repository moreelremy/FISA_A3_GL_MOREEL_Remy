/// <summary>
/// Gère la vue
/// </summary>
class View
{
    /// <summary>
    /// Affiche le menu (choix pour l'utilisateur)
    /// </summary>
    /// <returns>Le numéro saisie par l'utilisateur</returns>

    public static string ShowMenu()
    {
        Console.WriteLine("______________________________");
        Console.WriteLine("[1]: Créer Sauvegarde");
        Console.WriteLine("[2]: Lancer une sauvegarde");
        Console.WriteLine("[3]: Consulter log");
        Console.WriteLine("[4]: Changer de langue");
        Console.WriteLine("[5]: Quitter l'application");
        Console.WriteLine("______________________________");


        return InputHelper.ReadLineNotNull("Veuillez entrer un numéro !  ");

    }
}


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
        Console.WriteLine("[1]: feature 1");
        Console.WriteLine("[2]: feature 2");
        Console.WriteLine("[3]: feature 3 ...");
        Console.WriteLine("[4]: Quitter l'application");
        Console.WriteLine("______________________________");


        return InputHelper.ReadLineNotNull("Veuillez entrer un numéro !  ");

    }
}
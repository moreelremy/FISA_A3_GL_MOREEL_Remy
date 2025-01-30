
/// <summary>
/// Permet gérer les entrées utilisateur dans la console.
/// Evite de faire un Console.WriteLine("......") + Console.ReadLine() + Evite les message vide
/// </summary>
class InputHelper
{
    /// <summary>
    /// Lit une entrée utilisateur non vide à partir de la console.
    /// </summary>
    /// <param name="message">Le message à afficher pour demander l'entrée à l'utilisateur.</param>
    /// <returns>La chaîne saisie par l'utilisateur, qui ne peut pas être vide ou composée uniquement d'espaces.</returns>
    public static string ReadLineNotNull(string message)
    {
        string input = "";
        Console.WriteLine(message);
        input = Console.ReadLine();

        while (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("L'entrée ne peut pas être vide. Veuillez entrer une valeur valide.");
            input = Console.ReadLine();
        }
        return input.Trim();
    }

}


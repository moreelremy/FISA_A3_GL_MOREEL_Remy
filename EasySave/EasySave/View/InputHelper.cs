/// <summary>
/// Allows you to manage user input in the console.
/// Avoid doing a Console.WriteLine("......") + Console.ReadLine() + Avoid empty messages
/// </summary>
class InputHelper
{
    /// <summary>
    /// Reads non-empty user input from the console.
    /// </summary>
    /// <param name="message">The message to display to request input from the user.</param>
    /// <returns>The string entered by the user, which cannot be empty or all spaces.</returns>
    public static string ReadLineNotNull(string message, bool allowReturnToMenu = true)
    {
        Console.WriteLine(message);
        string? input = Console.ReadLine();

        while (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine(Language.GetString("InputError"));
            input = Console.ReadLine();
        }
        // If the user enters "9", throw an exception to return to the menu
        if (allowReturnToMenu && input.Trim() == "9")
        {
            throw new ReturnToMenuException();
        }
        return input.Trim();
    }

}
/// <summary>
/// Exception to handle returning to the main menu.
/// </summary>
public class ReturnToMenuException : Exception
{
    public ReturnToMenuException() : base(Language.GetString("ReturningToMenu")) { }
}
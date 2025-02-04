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
    public static string ReadLineNotNull(string message)
    {
        Console.WriteLine(message);
        string input = Console.ReadLine();

        while (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine(Language.GetString("InputError"));
            input = Console.ReadLine();
        }
        return input.Trim();
    }

}
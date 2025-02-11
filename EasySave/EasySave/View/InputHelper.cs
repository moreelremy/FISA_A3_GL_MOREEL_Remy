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
        string input = "";

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

            // Check if Escape key is pressed
            if (allowReturnToMenu && keyInfo.Key == ConsoleKey.Escape)
            {
                throw new ReturnToMenuException();
            }

            // If Enter is pressed and input is not empty, return the input
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                if (!string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine(); // Move to next line
                    return input.Trim();
                }
                Console.WriteLine(Language.GetString("InputHelper_InputError"));
                input = "";
                continue;
            }

            // Append character to input and display it
            input += keyInfo.KeyChar;
            Console.Write(keyInfo.KeyChar);
        }
    }
}

/// <summary>
/// Exception to handle returning to the main menu.
/// </summary>
public class ReturnToMenuException : Exception
{
    public ReturnToMenuException() : base(Language.GetString("InputHelper_ReturningToMenu")) { }
}

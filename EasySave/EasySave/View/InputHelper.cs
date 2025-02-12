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
        string? input = ReadInputWithEscapeCheck(allowReturnToMenu);

        while (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine(Language.GetString("InputHelper_InputError"));
            input = ReadInputWithEscapeCheck(allowReturnToMenu);
        }

        return input.Trim();
    }

    /// <summary>
    /// Reads user input while checking for the Escape key to return to the menu.
    /// Ensures the input is displayed properly without being erased.
    /// </summary>
    /// <returns>The user input string.</returns>
    private static string? ReadInputWithEscapeCheck(bool allowReturnToMenu)
    {
        string input = "";
        ConsoleKeyInfo keyInfo;

        do
        {
            keyInfo = Console.ReadKey(intercept: true);

            if (allowReturnToMenu && keyInfo.Key == ConsoleKey.Escape)
            {
                throw new ReturnToMenuException();
            }

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                break;
            }

            if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input.Substring(0, input.Length - 1);
                Console.Write("\b \b"); // Erase the last character properly
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                input += keyInfo.KeyChar;
                Console.Write(keyInfo.KeyChar);
            }
        } while (true);

        Console.WriteLine(); // Move to the next line after Enter
        return input;
    }
}

/// <summary>
/// Exception to handle returning to the main menu.
/// </summary>
public class ReturnToMenuException : Exception
{
    public ReturnToMenuException() : base(Language.GetString("InputHelper_ReturningToMenu")) { }
}
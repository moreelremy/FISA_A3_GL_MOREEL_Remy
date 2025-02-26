/// <summary>
/// Allows you to manage user input in the console.
/// Avoid doing a Console.WriteLine("......") + Console.ReadLine() + Avoid empty messages
/// </summary>
public class InputHelper
{
    /// <summary>
    /// Reads non-empty user input from the console.
    /// </summary>
    /// <param name="message">The message to display to request input from the user.</param>
    /// <returns>The string entered by the user, which cannot be empty or all spaces.</returns>
    public static string ReadLine(bool allowReturnToMenu = true, bool lineNotNull = true)
    {
        string? input = "";
        ConsoleKeyInfo keyInfo;

        do
        {
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

            if (!string.IsNullOrWhiteSpace(input) || !lineNotNull)
            {
                break;
            }

            Console.WriteLine(Language.GetString("InputHelper_InputError"));

        } while (true);

        return input.Trim();
    }
}

/// <summary>
/// Exception to handle returning to the main menu.
/// </summary>
public class ReturnToMenuException : Exception
{
    public ReturnToMenuException() : base(Language.GetString("InputHelper_ReturningToMenu")) { }
}
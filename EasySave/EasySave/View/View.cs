using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System;

public interface IView
{
    string ShowMenu();
    string GetLanguageChoice();
    void SaveAddedMessageView(Save save);
    void ShowSavesView(List<Save> saves);
    void DisplaySavesForExecution(List<Save> saves);
    List<int> GetSaveSelection(int maxCount);
    void DisplayExecutionResult(bool success);
    string ShowChoiceMenuOrDelete();
    void DisplaySavesForDeletion(List<Save> saves);
    int GetSaveIndexForDeletion(int maxIndex);
    void DisplayDeleteResult(bool isDeleted);
    void DisplaySuccess(string message);
    void DisplayError(string message);
    Dictionary<string, string> CreateBackupView();
    string GetWantedDate();
    void PromptToContinue();
    void DisplayLog(Dictionary<string, object> log);
    void Output(string? output);
    string? Input(bool allowReturnToMenu = true, bool LineNotNull = true);
}

/// <summary>
/// Manage default View (Console App for user) 
/// </summary>
class ViewBasic : IView
{
    /// <summary>
    /// Displays the menu (choice for the user)
    /// </summary>
    /// <returns>The number entered by the user</returns>
    public string ShowMenu()
    {
        Output("╔═════════════════════════════════════╗");
        Output("║              Easy Save              ║");
        Output("╚═════════════════════════════════════╝");
        Output($"    [1]: {Language.GetString("View_CreateBackup")}");
        Output($"    [2]: {Language.GetString("View_StartBackup")}");
        Output($"    [3]: {Language.GetString("View_ViewAllSaves")}");
        Output($"    [4]: {Language.GetString("ControllerView_ViewLogs")}");
        Output($"    [5]: {Language.GetString("View_ChangeLanguage")}");
        Output($"    [6]: {Language.GetString("View_ExitApp")}\n\n");
        Output(Language.GetString("View_EnterNumber"));

        return Input(allowReturnToMenu: false);
    }

    /// <summary>
    /// Ask the user to choose a language
    /// </summary>
    /// <returns>The selected language code (EN / FR / ..)</returns>
    public string GetLanguageChoice()
    {
        Output(Language.GetString("View_LanguageChoice"));
        return Input();
    }

    /// <summary>
    /// Displays that the backup has been created and takes the list as a parameter
    /// </summary>
    /// <param name="save">Set up a backup list</param>
    public void SaveAddedMessageView(Save save)
    {
        Output(string.Format(Language.GetString("View_SaveAddedMessage"), save.name));
    }

    /// <summary>
    /// Displays the list of backups with their number in the list
    /// </summary>
    /// <param name="saves">Set up a backup list</param>
    public void ShowSavesView(List<Save> saves)
    {
        Output("╔═══════════════════════════════════╗");
        Output(Language.GetString("View_ListOfBackups"));
        Output("╚═══════════════════════════════════╝");
        for (int i = 0; i < saves.Count; i++)
        {
            Output($"   {Language.GetString("View_NumberSave")} : [{i + 1}]");
            Output($"   " + Language.GetString("View_SaveName") + $" : {saves[i].name}");
            Output($"   " + Language.GetString("View_SaveSource") + $" : {saves[i].sourceDirectory}");
            Output($"   " + Language.GetString("View_SaveTarget") + $" : {saves[i].targetDirectory}");
            Output($"   " + Language.GetString("View_SaveType") + $" : {Language.GetString($"View_{saves[i].saveStrategy.GetType().Name}")}");
            Output($"   " + Language.GetString("View_logFileExtension") + $" : {saves[i].logFileExtension}");
            Output("═════════════════════════════════════");
        }

    }

    public void DisplaySavesForExecution(List<Save> saves)
    {
        Output(Language.GetString("View_ChooseSaveToExecute"));
        for (int i = 0; i < saves.Count; i++)
        {
            Output($"[{i + 1}] {saves[i].name}");
        }
    }

    /// <summary>
    /// Parses a user input string representing selected indices or index ranges and converts them into a list of zero-based indices.
    /// </summary>
    /// <param name="input">A string containing individual indices or ranges separated by semicolons (e.g., "1;3-5;7").</param>
    /// <param name="maxCount">The maximum allowed index (1-based), ensuring selections stay within valid bounds.</param>
    /// <returns>
    /// A list of integers representing the zero-based indices of the selected items.
    /// If the input is invalid, an empty list is returned.
    /// </returns>
    private List<int> ParseSaveSelection(string input, int maxCount)
    {
        List<int> selectedIndexes = new List<int>();

        try
        {
            string[] ranges = input.Split(';');
            foreach (string range in ranges)
            {
                if (range.Contains('-'))
                {
                    string[] bounds = range.Split('-');
                    int start = int.Parse(bounds[0]) - 1;
                    int end = int.Parse(bounds[1]) - 1;

                    for (int i = start; i <= end && i < maxCount; i++)
                    {
                        if (!selectedIndexes.Contains(i))
                            selectedIndexes.Add(i);
                    }
                }
                else
                {
                    int index = int.Parse(range) - 1;
                    if (index >= 0 && index < maxCount)
                    {
                        selectedIndexes.Add(index);
                    }
                }
            }
        }
        catch
        {
            Output(Language.GetString("View_InvalidSelection"));
        }

        return selectedIndexes;
    }

    /// <summary>
    /// Prompts the user to enter save IDs to execute and parses them into a list of zero-based indices.
    /// </summary>
    /// <param name="maxCount">The maximum number of selectable save entries.</param>
    /// <returns>A list of integers representing the selected zero-based indices.</returns>
    public List<int> GetSaveSelection(int maxCount)
    {
        Output(Language.GetString("View_EnterSaveIdsToExecute"));
        string input = Input();
        return ParseSaveSelection(input, maxCount);
    }

    /// <summary>
    /// Displays the execution result message based on success or failure.
    /// </summary>
    /// <param name="success">A boolean indicating whether the execution was successful.</param>
    public void DisplayExecutionResult(bool success)
    {
        Output(success ? Language.GetString("View_ExecutionCompleted") : Language.GetString("View_ExecutionFailed"));
    }

    /// <summary>
    /// Shows the choice between returning to the menu or deleting a save
    /// </summary>
    /// <returns>returns the user's choice to use it in the switch case</returns>
    public string ShowChoiceMenuOrDelete()
    {
        Output(Language.GetString("Controller_ChoiceDeleteOrMenu") + "\n\n" +
               "[1] : " + Language.GetString("Controller_ChoiceMenu") + "\n" +
               "[2] : " + Language.GetString("Controller_ChoiceDelete"));
        string choice = Input();
        return choice;
    }

    /// <summary>
    /// Displays a list of saves with corresponding numbers.
    /// </summary>
    /// <param name="saves">List of saves to display.</param>
    public void DisplaySavesForDeletion(List<Save> saves)
    {
        Output(Language.GetString("View_ChooseSaveToDelete"));

        for (int i = 0; i < saves.Count; i++)
        {
            Output($"[{i + 1}] {saves[i].name}");
        }

        Output("");
    }

    /// <summary>
    /// Reads the user's choice of save to delete.
    /// </summary>
    /// <returns>The chosen save index (0-based).</returns>
    public int GetSaveIndexForDeletion(int maxIndex)
    {
        Output(Language.GetString("View_EnterSaveNumber"));
        string input = Input();
        if (int.TryParse(input, out int index) && index > 0 && index <= maxIndex)
        {
            return index - 1;  // Convert to 0-based index
        }

        Output(Language.GetString("Controller_InvalidChoice"));
        return -1;  // Invalid choice
    }

  


    /// <summary>
    /// Displays the result of the deletion.
    /// </summary>
    /// <param name="isDeleted">True if the save was deleted, otherwise false.</param>
    public void DisplayDeleteResult(bool isDeleted)
    {
        if (isDeleted)
        {
            Output(Language.GetString("View_SaveDeleted"));
        }
        else
        {
            Output(Language.GetString("View_SaveNotFound"));
        }
    }

    /// <summary>
    /// Displays a success message in green.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public void DisplaySuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Output(message);
        Console.ResetColor();
    }

    /// <summary>
    /// Displays an error message in red.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public void DisplayError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Output(message);
        Console.ResetColor();
    }

    /// <summary>
    /// Collects information from the user to create a new backup, With a return to menu option
    /// </summary>
    /// <returns>The newly created Save object</returns>
    public Dictionary<string, string> CreateBackupView()
    {
        Output(Language.GetString("View_EnterBackupName"));
        string name = Input();
        Output(Language.GetString("View_EnterSourcePath"));
        string source = Input();
        Output(Language.GetString("View_EnterTargetPath"));
        string target = Input();

        Output("[1] " + Language.GetString("View_FullSave"));
        Output("[2] " + Language.GetString("View_DifferentialSave"));
        string saveStrategy;
        while (true)
        {
            Output(Language.GetString("View_SelectBackupType"));
            saveStrategy = Input();
            if (saveStrategy == "1" || saveStrategy == "2")
            {
                break;
            }
            else
            {
                Output(Language.GetString("View_InvalidSelection")); // Display error message
            }
        }

        Output("[1] " + Language.GetString("View_jsonFileExtension"));
        Output("[2] " + Language.GetString("View_xmlFileExtension"));
        string logFileExtension;
        while (true)
        {
            Output(Language.GetString("View_SelectBackupType"));
            logFileExtension = Input();
            if (logFileExtension == "1" || logFileExtension == "2")
            {
                break;
            }
            else
            {
                Output(Language.GetString("View_InvalidSelection")); // Display error message
            }
        }

        switch (logFileExtension)
        {
            case "1":
                logFileExtension = "json";
                break;

            case "2":
                logFileExtension = "xml";
                break;

            default:
                logFileExtension = "json";
                break;
        }

        Dictionary<string, string> result = new Dictionary<string, string>();
        result.Add("name", name);
        result.Add("sourceDirectory", source);
        result.Add("targetDirectory", target);
        result.Add("saveStrategy", saveStrategy);
        result.Add("logFileExtension", logFileExtension);
        return result;
    }

    /// <summary>
    /// Ask the user to choose a date for listing logs
    /// </summary>
    /// <returns>The date dd-mm-yyyy</returns>
    public string GetWantedDate()
    {
        Output(Language.GetString("View_DateChoice"));
        string? result = Input(lineNotNull: false);

        return result == "" ? $"{DateTime.Now:dd-MM-yyyy}" : result;
    }

    /// <summary>
    /// Prompts the user to press any key to continue.
    /// </summary>
    public void PromptToContinue()
    {
        Output(Language.GetString("Controller_PressAnyKey"));
        Input(allowReturnToMenu: false, lineNotNull: false);
        Console.Clear();
    }

    /// <summary>
    /// Affiche les informations d'un log dans la console.
    /// </summary>
    /// <param name="log">L'entrée de log à afficher.</param>
    public void DisplayLog(Dictionary<string, object> log)
    {
        if (log == null)
        {
            Output("LogEntry is null.");
            return;
        }

        Output($"Timestamp: {log["timestamp"]}");
        Output($"SaveName: {log["saveName"]}");
        Output($"Source: {log["source"]}");
        Output($"Target: {log["target"]}");
        Output($"Size: {log["size"]}");
        Output($"TransferTimeMs: {log["transferTimeMs"]}");
        Output("══════════════════════════════");
    }

    /// <summary>
    /// Just print a message in the console
    /// </summary>
    /// <param name="output">Take the sentence to display as a parameter</param>
    public virtual void Output(string? output)
    {
        Console.WriteLine(output);
    }

    public virtual string? Input(bool allowReturnToMenu = true, bool lineNotNull = true)
    {
        return InputHelper.ReadLine(allowReturnToMenu, lineNotNull);
    }
}

/// <summary>
/// Inherit the Basic View to manage a server/client app
/// </summary>
class ViewServer : ViewBasic
{
    private Socket _serverSocket;
    private Socket _clientSocket;
    ViewServer()
    {
        try
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 5000);
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(endPoint);
            _serverSocket.Listen(1);
            _clientSocket = _serverSocket.Accept();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Environment.Exit(0);
        }
    }

    override public void Output(string? output)
    {
        _clientSocket.Send(Encoding.UTF8.GetBytes(output));
    }

    public override string? Input(bool allowReturnToMenu = true, bool lineNotNull = true)
    {
        Output(JsonSerializer.Serialize(
        new
        {
            Input = (allowReturnToMenu, lineNotNull)
        }));
        byte[] buffer = new byte[1024];
        int bytesRead;
        string message = "";
        while ((bytesRead = _clientSocket.Receive(buffer)) > 0)
        {
            message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }
        return message;
    }
}
using System.Diagnostics;
using System.Text.Json;
using CryptoSoft;

/// <summary>
/// Represents a backup save operation, containing details about the source, target, and strategy used.
/// </summary>
public class Save
{
    public int DisplayIndex { get; set; }
    public required string name { get; set; }
    public required string sourceDirectory { get; set; }
    public required string targetDirectory { get; set; }
    public required SaveStrategy saveStrategy { get; set; }
    public required string logFileExtension { get; set; }
}

public class SaveStrategyFactory
{
    public SaveStrategy CreateSaveStrategy(string saveStrategy)
    {
        switch (saveStrategy)
        {
            case "1" or "FullSave":
                return new FullSave();

            case "2" or "DifferentialSave":
                return new DifferentialSave();

            default:
                return new FullSave();
        }
    }
}

public abstract class SaveStrategy
{
    public void commonSave(Save save, int totalFilesToCopy, long totalFileSize, DateTime? lastChangeDateTime = null){
        string target = @"\" + save.name + @"\" + DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss.ff");
        DateTime startSave = DateTime.UtcNow;
        int encryptionTime;
        encryptionTime = commonSaveDirectory(save.sourceDirectory, string.Concat(save.targetDirectory, target), save.name, totalFilesToCopy, totalFileSize, totalFilesToCopy, totalFileSize, save.logFileExtension, 0);
        // Log the end of the save in the real-time log
        Logs.RealTimeLog(
            saveName: save.name,
            sourcePath: "",
            targetPath: "",
            fileSize: 0,
            state: "END",
            totalFilesToCopy: 0,
            totalFileSize: 0,
            nbFilesLeftToDo: 0,
            filesSizeLeftToDo: 0,
            Progression: 0,
            logFileExtension: save.logFileExtension
        );
        DateTime endSave = DateTime.UtcNow;
        // Calculate the time taken to save
        int transferTime = (int)(endSave - startSave).TotalMilliseconds;
        Logs.GeneralLog(save, totalFileSize, transferTime, encryptionTime);
    }

    /// <summary>
    /// Executes the save process using the provided save configuration.
    /// </summary>
    /// <param name="save">The save instance containing backup details.</param>
    public abstract void Save(Save save);

    public int commonSaveDirectory(string sourceDirectory, string targetDirectory, string saveName, int totalFilesToCopy, long totalFileSize, int nbFilesLeftToDo, long filesSizeLeftToDo, string logFileExtension, int encryptionTime, DateTime? lastChangeDateTime = null)
    {
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        string settingsJson = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "../../../../settings.json"));
        var settings = JsonSerializer.Deserialize<Dictionary<string, object>>(settingsJson);


        // TO MODIFY : avoid creating a directory if the calculator is launched from the start
        // ADD : reading the settings json file for the name of the business CalculatorApp
        //string calculatorProcessName = settings["UserInputSettingsSoftware"];
        string processName = settings["UserInputSettingsSoftware"].ToString();
        var processes = Process.GetProcesses();


        // Copy all files in the source directory to the target directory
        foreach (string file in Directory.GetFiles(sourceDirectory))
        {
            bool isProcessRunning = processes.Any(p => p.ProcessName.ToLower() == processName.ToLower());

            if (!isProcessRunning)
            {
                string target = Path.Combine(targetDirectory, Path.GetFileName(file));
                // Copy the file if it has been modified since the last save
                if (lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime)
                {
                    File.Copy(file, target, true);


                    if (true)// to replace check si extension
                    {
                        DateTime startFilencryption = DateTime.UtcNow;
                        Crypt.Encrypt(target, "02e5d449168bb31da11145d04d6da992ffc7f8f20c04dcf5a046f7620ee6236");
                        DateTime stopFilencryption = DateTime.UtcNow;
                        encryptionTime += (int)(stopFilencryption - startFilencryption).TotalMilliseconds;
                    }
                    long fileSize = new FileInfo(file).Length;
                    nbFilesLeftToDo -= 1;
                    filesSizeLeftToDo -= fileSize;
                    // Log the file copy in the real-time log
                    Logs.RealTimeLog(
                        saveName: saveName,
                        sourcePath: file,
                        targetPath: target,
                        fileSize: fileSize,
                        state: "ACTIVE",
                        totalFilesToCopy: totalFilesToCopy,
                        totalFileSize: totalFileSize,
                        nbFilesLeftToDo: nbFilesLeftToDo,
                        filesSizeLeftToDo: filesSizeLeftToDo,
                        Progression: (int)(((float)totalFileSize - (float)filesSizeLeftToDo) / (float)totalFileSize * 100),
                        logFileExtension: logFileExtension
                    );
                }
            }
            else
            {
                throw new InvalidOperationException("Le processus métier est en cours d'exécution. Opération annulée.");

            }
        }

        // Copy all subdirectories in the source directory to the target directory
        foreach (string directory in Directory.GetDirectories(sourceDirectory))
        {
            string target = Path.Combine(targetDirectory, Path.GetFileName(directory));
            encryptionTime = commonSaveDirectory(directory, target, saveName, totalFilesToCopy, totalFileSize, nbFilesLeftToDo, filesSizeLeftToDo, logFileExtension, encryptionTime, lastChangeDateTime);
            (nbFilesLeftToDo, filesSizeLeftToDo) = SaveDirectory(target, nbFilesLeftToDo, filesSizeLeftToDo, lastChangeDateTime);
        }

        return encryptionTime;
    }

    /// <summary>
    /// Saves a directory by copying files and subdirectories.
    /// </summary>
    public abstract (int,long) SaveDirectory(string target, int nbFilesLeftToDo, long filesSizeLeftToDo, DateTime? lastChangeDateTime = null);
}
 
/// <summary>
/// Implements a full save strategy that copies all files and directories.
/// </summary>
public class FullSave : SaveStrategy
{
    /// <inheritdoc/>
    public override void Save(Save save)
    {
        try
        {
            // Count the number of files to copy and the total size of the files
            int totalFilesToCopy = Directory.GetFiles(save.sourceDirectory, "*.*", SearchOption.AllDirectories).Count();
            // Sum the size of all files in the source directory
            long totalFileSize = Directory.GetFiles(save.sourceDirectory, "*.*", SearchOption.AllDirectories).Sum(file => new FileInfo(file).Length);
            commonSave(save, totalFilesToCopy, totalFileSize);
        }
        catch (DirectoryNotFoundException directoryNotFound)
        {
            Console.WriteLine("The source directory path of the save is valid but does not exist." + directoryNotFound.Message);
        }
        catch (InvalidOperationException message)
        {
            throw new InvalidOperationException(message.Message);
        }
        catch
        {
            Console.WriteLine("The source directory path of the save is invalid or you don't have the required access.");
        }
    }

    /// <inheritdoc/>
    public override (int, long) SaveDirectory(string target, int nbFilesLeftToDo, long filesSizeLeftToDo, DateTime? lastChangeDateTime = null)
    {
        nbFilesLeftToDo -= Directory.GetFiles(target, "*.*", SearchOption.AllDirectories).Count();
        filesSizeLeftToDo -= Directory.GetFiles(target, "*.*", SearchOption.AllDirectories).Sum(file => new FileInfo(file).Length);
        return (nbFilesLeftToDo, filesSizeLeftToDo);
    }
}

/// <summary>
/// Implements a differential save strategy that only copies changed files since the last save.
/// </summary>
public class DifferentialSave : SaveStrategy
{
    /// <inheritdoc/>
    public override void Save(Save save)
    {
        try
        {
            DateTime? lastChangeDateTime = null;
            string targetRepo = string.Concat(save.targetDirectory, @"\" + save.name);
            // Get the last save date time if exists
            if (Directory.Exists(targetRepo))
            {
                lastChangeDateTime = Directory.GetCreationTime(targetRepo);
            }
            // Count the number of files to copy and the total size of the files
            string[] filesToCopy = Directory.GetFiles(save.sourceDirectory, "*.*", SearchOption.AllDirectories);
            int totalFilesToCopy = filesToCopy.Where(file => lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime).Count();
            long totalFileSize = filesToCopy.Where(file => lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime).Sum(file => new FileInfo(file).Length);
            commonSave(save, totalFilesToCopy, totalFileSize, lastChangeDateTime);
        }
        catch (DirectoryNotFoundException directoryNotFound)
        {
            Console.WriteLine("The source directory path of the save is valid but does not exist." + directoryNotFound.Message);
        }
        catch
        {
            Console.WriteLine("The source directory path of the save is invalid or you don't have the required access.");
        }
    }

    /// <inheritdoc/>
    public override (int, long) SaveDirectory(string target, int nbFilesLeftToDo, long filesSizeLeftToDo, DateTime? lastChangeDateTime = null)
    {
        string[] filesToCopy = Directory.GetFiles(target, "*.*", SearchOption.AllDirectories);
        nbFilesLeftToDo -= filesToCopy.Where(file => lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime).Count();
        filesSizeLeftToDo -= filesToCopy.Where(file => lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime).Sum(file => new FileInfo(file).Length);
        return (nbFilesLeftToDo, filesSizeLeftToDo);
    }
}
using System.Diagnostics;
using System.Text.Json;
using CryptoSoft;

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
            case "1":
            case "FullSave":
                return new FullSave();

            case "2":
            case "DifferentialSave":
                return new DifferentialSave();

            default:
                return new FullSave();
        }
    }
}

public abstract class SaveStrategy
{
    /// <summary>
    /// Save execution with cancellation, pause/resume and progress reporting.
    /// </summary>
    public abstract void Save(Save save, CancellationToken token, ManualResetEventSlim pauseEvent, Action<int> progressCallback);

    /// <summary>
    /// Helper method for synchronous save. Creates a unique target folder,
    /// performs the file copy/encryption, logs the real-time progress and then the summary log.
    /// </summary>
    public void commonSave(Save save, CancellationToken token, ManualResetEventSlim pauseEvent, Action<int> progressCallback, DateTime? lastChangeDateTime)
    {
        string target = @"\" + save.name + @"\" + DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss.ff");
        List<(string, string)> filesToCopy = fetchFilesFromDirectory(save.name, save.sourceDirectory, string.Concat(save.targetDirectory, target), lastChangeDateTime);

        var settings = Data.LoadFromJson(Path.Combine(Directory.GetCurrentDirectory(), "../../../../settings.json"));
        string processName = settings["SettingsSoftware"].ToString();
        var extensionsJson = (JsonElement)settings["ExtensionsToCrypt"];
        List<string> extensionsToCrypt = extensionsJson.EnumerateArray().Select(e => e.GetString()).ToList();
        extensionsJson = (JsonElement)settings["ExtensionsToPrioritize"];
        List<string> extensionsToPrioritize = extensionsJson.EnumerateArray().Select(e => e.GetString()).ToList();

        List<(string, string)> sortedFilesToCopy = filesToCopy
            .OrderBy(f => extensionsToPrioritize.Contains(Path.GetExtension(f.Item1).TrimStart('.')) ? extensionsToPrioritize.IndexOf(Path.GetExtension(f.Item1).TrimStart('.')) : int.MaxValue)
            .ToList();

        // Log information init
        DateTime startSave = DateTime.UtcNow;
        int totalFilesToCopy = sortedFilesToCopy.Count();
        long totalFileSize = sortedFilesToCopy.Sum(file => new FileInfo(file.Item1).Length);
        int nbFilesLeftToDo = sortedFilesToCopy.Count();
        long filesSizeLeftToDo = sortedFilesToCopy.Sum(file => new FileInfo(file.Item1).Length);
        int encryptionTime = 0;

        foreach (var file in sortedFilesToCopy)
        {
            token.ThrowIfCancellationRequested(); // Check if the process is cancelled
            pauseEvent.Wait(token); // Pause execution if required

            if (Process.GetProcesses().Any(p => p.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase)))
            {
                pauseEvent.Reset();
                while (Process.GetProcesses().Any(p => p.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase)))
                {
                    //Do nothing
                }
            }

            pauseEvent.Set(); // Resume save process

            File.Copy(file.Item1, file.Item2, true);

            string fileExtension = Path.GetExtension(file.Item2).TrimStart('.');

            // Encrypt if required
            if (extensionsToCrypt.Contains(fileExtension))
            {
                DateTime startFilencryption = DateTime.UtcNow;
                Crypt.Encrypt(file.Item2, "02e5d449168bb31da11145d04d6da992ffc7f8f20c04dcf5a046f7620ee6236");
                DateTime stopFilencryption = DateTime.UtcNow;
                encryptionTime += (int)(stopFilencryption - startFilencryption).TotalMilliseconds;
            }

            long fileSize = new FileInfo(file.Item1).Length;
            nbFilesLeftToDo -= 1;
            filesSizeLeftToDo -= fileSize;

            // Compute progress
            int currentProgress = totalFileSize > 0 ? (int)(((float)(totalFileSize - filesSizeLeftToDo) / totalFileSize) * 100) : 100;
            progressCallback?.Invoke(currentProgress); // Update UI progress

            Data.RealTimeLog(
                saveName: save.name,
                sourcePath: file.Item1,
                targetPath: file.Item2,
                fileSize: fileSize,
                state: "ACTIVE",
                totalFilesToCopy: totalFilesToCopy,
                totalFileSize: totalFileSize,
                nbFilesLeftToDo: nbFilesLeftToDo,
                filesSizeLeftToDo: filesSizeLeftToDo,
                Progression: currentProgress,
                logFileExtension: save.logFileExtension
            );
        }
        // Log the end of the save operation.
        Data.RealTimeLog(
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
        int transferTime = (int)(endSave - startSave).TotalMilliseconds;
        Data.GeneralLog(save, totalFileSize, transferTime, encryptionTime);
    }

    /// <summary>
    /// Helper method to copy files supporting cancellation, pause/resume, and progress reporting.
    /// </summary>
    public List<(string, string)> fetchFilesFromDirectory(string saveName, string sourceDirectory, string targetDirectory, DateTime? lastChangeDateTime = null)
    {
        List<(string, string)> filesToCopy = new List<(string, string)>();
        try
        {
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            // Get all files from the source directory
            var files = Directory.GetFiles(sourceDirectory);
            foreach (string file in files)
            {
                if (lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime)
                {
                    string targetFile;
                    targetFile = Path.Combine(targetDirectory, Path.GetFileName(file));
                    filesToCopy.Add((file, targetFile));
                }
            }

            // Process subdirectories recursively
            foreach (string directory in Directory.GetDirectories(sourceDirectory))
            {
                string target = Path.Combine(targetDirectory, Path.GetFileName(directory));
                if (directory != Path.Combine(sourceDirectory, saveName)) // Prevent recursive loop
                {
                    filesToCopy = filesToCopy.Concat(fetchFilesFromDirectory(
                        saveName,
                        directory,
                        target,
                        lastChangeDateTime
                    )).ToList();
                }
            }
        }
        catch (OperationCanceledException)
        {
            throw new Exception($"Save '{saveName}' was cancelled.");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        return filesToCopy;
    }
}

public class FullSave : SaveStrategy
{
    /// <inheritdoc/>
    public override void Save(Save save, CancellationToken token, ManualResetEventSlim pauseEvent, Action<int> progressCallback)
    {
        try
        {
            commonSave(save, token, pauseEvent, progressCallback, null);
        }
        catch (DirectoryNotFoundException directoryNotFound)
        {
            throw new DirectoryNotFoundException("The source directory path of the save is valid but does not exist. " + directoryNotFound.Message);
        }
        catch (InvalidOperationException message)
        {
            throw new InvalidOperationException(message.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}

public class DifferentialSave : SaveStrategy
{
    /// <inheritdoc/>
    public override void Save(Save save, CancellationToken token, ManualResetEventSlim pauseEvent, Action<int> progressCallback)
    {
        try
        {
            DateTime? lastChangeDateTime = null;
            string targetRepo = string.Concat(save.targetDirectory, @"\" + save.name);
            if (Directory.Exists(targetRepo))
            {
                lastChangeDateTime = Directory.GetCreationTime(targetRepo);
            }
            commonSave(save, token, pauseEvent, progressCallback, lastChangeDateTime);
        }
        catch (DirectoryNotFoundException directoryNotFound)
        {
            throw new DirectoryNotFoundException("The source directory path of the save is valid but does not exist. " + directoryNotFound.Message);
        }
        catch (InvalidOperationException message)
        {
            throw new InvalidOperationException(message.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
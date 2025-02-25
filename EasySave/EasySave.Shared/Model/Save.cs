using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
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
    /// Synchronous save execution.
    /// </summary>
    public abstract void Save(Save save);

    /// <summary>
    /// Save execution with cancellation, pause/resume and progress reporting.
    /// </summary>
    public abstract void Save(Save save, CancellationToken token, ManualResetEventSlim pauseEvent, Action<int> progressCallback);

    /// <summary>
    /// Helper method for synchronous save. Creates a unique target folder,
    /// performs the file copy/encryption, logs the real-time progress and then the summary log.
    /// </summary>
    public void commonSave(Save save, int totalFilesToCopy, long totalFileSize, DateTime? lastChangeDateTime = null)
    {
        string target = @"\" + save.name + @"\" + DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss.ff");
        DateTime startSave = DateTime.UtcNow;
        int encryptionTime = commonSaveDirectory(
            save.sourceDirectory,
            string.Concat(save.targetDirectory, target),
            save.name,
            totalFilesToCopy,
            totalFileSize,
            totalFilesToCopy,
            totalFileSize,
            save.logFileExtension,
            0,
            lastChangeDateTime
        );
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
    /// Synchronous helper method to copy files from source to target directory.
    /// </summary>
    public int commonSaveDirectory(
        string sourceDirectory,
        string targetDirectory,
        string saveName,
        int totalFilesToCopy,
        long totalFileSize,
        int nbFilesLeftToDo,
        long filesSizeLeftToDo,
        string logFileExtension,
        int encryptionTime,
        DateTime? lastChangeDateTime = null)
    {
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        var settings = Data.LoadFromJson(Path.Combine(Directory.GetCurrentDirectory(), "../../../../settings.json"));
        string processName = settings["UserInputSettingsSoftware"].ToString();
        var processes = Process.GetProcesses();
        var extensionsJson = (JsonElement)settings["ExtensionSelected"];
        List<string> extensions = extensionsJson.EnumerateArray().Select(e => e.GetString()).ToList();

        foreach (string file in Directory.GetFiles(sourceDirectory))
        {
            bool isProcessRunning = processes.Any(p => p.ProcessName.ToLower() == processName.ToLower());
            if (!isProcessRunning)
            {
                string target = Path.Combine(targetDirectory, Path.GetFileName(file));
                string fileExtension = Path.GetExtension(target).TrimStart('.');

                if (lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime)
                {
                    File.Copy(file, target, true);
                    if (extensions.Contains(fileExtension))
                    {
                        DateTime startFilencryption = DateTime.UtcNow;
                        Crypt.Encrypt(target, "02e5d449168bb31da11145d04d6da992ffc7f8f20c04dcf5a046f7620ee6236");
                        DateTime stopFilencryption = DateTime.UtcNow;
                        encryptionTime += (int)(stopFilencryption - startFilencryption).TotalMilliseconds;
                    }

                    long fileSize = new FileInfo(file).Length;
                    nbFilesLeftToDo -= 1;
                    filesSizeLeftToDo -= fileSize;
                    int currentProgress = (int)(((float)totalFileSize - (float)filesSizeLeftToDo) / totalFileSize * 100);
                    Data.RealTimeLog(
                        saveName: saveName,
                        sourcePath: file,
                        targetPath: target,
                        fileSize: fileSize,
                        state: "ACTIVE",
                        totalFilesToCopy: totalFilesToCopy,
                        totalFileSize: totalFileSize,
                        nbFilesLeftToDo: nbFilesLeftToDo,
                        filesSizeLeftToDo: filesSizeLeftToDo,
                        Progression: currentProgress,
                        logFileExtension: logFileExtension
                    );
                }
            }
            else
            {
                throw new InvalidOperationException("Le processus métier est en cours d'exécution. Opération annulée.");
            }
        }

        foreach (string directory in Directory.GetDirectories(sourceDirectory))
        {
            if (directory != Path.Combine(sourceDirectory, saveName))
            {
                string target = Path.Combine(targetDirectory, Path.GetFileName(directory));
                encryptionTime = commonSaveDirectory(
                    directory,
                    target,
                    saveName,
                    totalFilesToCopy,
                    totalFileSize,
                    nbFilesLeftToDo,
                    filesSizeLeftToDo,
                    logFileExtension,
                    encryptionTime,
                    lastChangeDateTime
                );
                (nbFilesLeftToDo, filesSizeLeftToDo) = SaveDirectory(target, nbFilesLeftToDo, filesSizeLeftToDo, lastChangeDateTime);
            }
        }

        return encryptionTime;
    }

    /// <summary>
    /// Helper method to copy files supporting cancellation, pause/resume, and progress reporting.
    /// </summary>
    public int commonSaveDirectory(
     string sourceDirectory,
     string targetDirectory,
     string saveName,
     int totalFilesToCopy,
     long totalFileSize,
     int nbFilesLeftToDo,
     long filesSizeLeftToDo,
     string logFileExtension,
     int encryptionTime,
     CancellationToken token,
     ManualResetEventSlim pauseEvent,
     Action<int> progressCallback,
     DateTime? lastChangeDateTime = null)
    {
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        var settings = Data.LoadFromJson(Path.Combine(Directory.GetCurrentDirectory(), "../../../../settings.json"));
        string processName = settings["UserInputSettingsSoftware"].ToString();
        var extensionsJson = (JsonElement)settings["ExtensionSelected"];
        List<string> extensions = extensionsJson.EnumerateArray().Select(e => e.GetString()).ToList();

        foreach (string file in Directory.GetFiles(sourceDirectory))
        {
            // See if the process is running and pause execution if necessary
            token.ThrowIfCancellationRequested();

            // Pause the save operation if the process is running
            while (Process.GetProcesses().Any(p => p.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase)))
            {
                pauseEvent.Reset();

                // Look every 100ms if the process is still running
                for (int i = 0; i < 20; i++) // 20 x 100ms = 2s
                {
                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(100);
                    if (!Process.GetProcesses().Any(p => p.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase)))
                        break;
                }
            }

            pauseEvent.Set(); // Reprendre la sauvegarde


            string target = Path.Combine(targetDirectory, Path.GetFileName(file));
            string fileExtension = Path.GetExtension(target).TrimStart('.');

            if (lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime)
            {
                File.Copy(file, target, true);
                if (extensions.Contains(fileExtension))
                {
                    DateTime startFilencryption = DateTime.UtcNow;
                    Crypt.Encrypt(target, "02e5d449168bb31da11145d04d6da992ffc7f8f20c04dcf5a046f7620ee6236");
                    DateTime stopFilencryption = DateTime.UtcNow;
                    encryptionTime += (int)(stopFilencryption - startFilencryption).TotalMilliseconds;
                }

                long fileSize = new FileInfo(file).Length;
                nbFilesLeftToDo -= 1;
                filesSizeLeftToDo -= fileSize;
                int currentProgress = (int)(((float)totalFileSize - (float)filesSizeLeftToDo) / totalFileSize * 100);

                // Reporter la progression
                progressCallback?.Invoke(currentProgress);

                Data.RealTimeLog(
                    saveName: saveName,
                    sourcePath: file,
                    targetPath: target,
                    fileSize: fileSize,
                    state: "ACTIVE",
                    totalFilesToCopy: totalFilesToCopy,
                    totalFileSize: totalFileSize,
                    nbFilesLeftToDo: nbFilesLeftToDo,
                    filesSizeLeftToDo: filesSizeLeftToDo,
                    Progression: currentProgress,
                    logFileExtension: logFileExtension
                );
            }
        }

        foreach (string directory in Directory.GetDirectories(sourceDirectory))
        {
            if (directory != Path.Combine(sourceDirectory, saveName))
            {
                string target = Path.Combine(targetDirectory, Path.GetFileName(directory));
                encryptionTime = commonSaveDirectory(
                    directory,
                    target,
                    saveName,
                    totalFilesToCopy,
                    totalFileSize,
                    nbFilesLeftToDo,
                    filesSizeLeftToDo,
                    logFileExtension,
                    encryptionTime,
                    token,
                    pauseEvent,
                    progressCallback,
                    lastChangeDateTime
                );
                (nbFilesLeftToDo, filesSizeLeftToDo) = SaveDirectory(target, nbFilesLeftToDo, filesSizeLeftToDo, lastChangeDateTime);
            }
        }

        return encryptionTime;
    }


    /// <summary>
    /// Saves a directory by copying files and subdirectories.
    /// </summary>
    public abstract (int, long) SaveDirectory(string target, int nbFilesLeftToDo, long filesSizeLeftToDo, DateTime? lastChangeDateTime = null);
}

public class FullSave : SaveStrategy
{
    /// <inheritdoc/>
    public override void Save(Save save)
    {
        try
        {
            int totalFilesToCopy = Directory.GetFiles(save.sourceDirectory, "*.*", SearchOption.AllDirectories).Count();
            long totalFileSize = Directory.GetFiles(save.sourceDirectory, "*.*", SearchOption.AllDirectories)
                .Sum(file => new FileInfo(file).Length);
            commonSave(save, totalFilesToCopy, totalFileSize);
        }
        catch (DirectoryNotFoundException directoryNotFound)
        {
            Console.WriteLine("The source directory path of the save is valid but does not exist. " + directoryNotFound.Message);
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
    public override void Save(Save save, CancellationToken token, ManualResetEventSlim pauseEvent, Action<int> progressCallback)
    {
        try
        {
            int totalFilesToCopy = Directory.GetFiles(save.sourceDirectory, "*.*", SearchOption.AllDirectories).Count();
            long totalFileSize = Directory.GetFiles(save.sourceDirectory, "*.*", SearchOption.AllDirectories)
                .Sum(file => new FileInfo(file).Length);
            string target = @"\" + save.name + @"\" + DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss.ff");
            DateTime startSave = DateTime.UtcNow;
            int encryptionTime = commonSaveDirectory(
                save.sourceDirectory,
                string.Concat(save.targetDirectory, target),
                save.name,
                totalFilesToCopy,
                totalFileSize,
                totalFilesToCopy,
                totalFileSize,
                save.logFileExtension,
                0,
                token,
                pauseEvent,
                progressCallback
            );
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
        catch (OperationCanceledException)
        {
            Console.WriteLine("Save operation was cancelled.");
            throw;
        }
        catch (DirectoryNotFoundException directoryNotFound)
        {
            Console.WriteLine("The source directory path of the save is valid but does not exist. " + directoryNotFound.Message);
        }
        catch (InvalidOperationException message)
        {
            throw new InvalidOperationException(message.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("The source directory path of the save is invalid or you don't have the required access. " + ex.Message);
        }
    }

    /// <inheritdoc/>
    public override (int, long) SaveDirectory(string target, int nbFilesLeftToDo, long filesSizeLeftToDo, DateTime? lastChangeDateTime = null)
    {
        nbFilesLeftToDo -= Directory.GetFiles(target, "*.*", SearchOption.AllDirectories).Count();
        filesSizeLeftToDo -= Directory.GetFiles(target, "*.*", SearchOption.AllDirectories)
            .Sum(file => new FileInfo(file).Length);
        return (nbFilesLeftToDo, filesSizeLeftToDo);
    }
}

public class DifferentialSave : SaveStrategy
{
    /// <inheritdoc/>
    public override void Save(Save save)
    {
        try
        {
            DateTime? lastChangeDateTime = null;
            string targetRepo = string.Concat(save.targetDirectory, @"\" + save.name);
            if (Directory.Exists(targetRepo))
            {
                lastChangeDateTime = Directory.GetCreationTime(targetRepo);
            }
            string[] filesToCopy = Directory.GetFiles(save.sourceDirectory, "*.*", SearchOption.AllDirectories);
            int totalFilesToCopy = filesToCopy
                .Where(file => lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime)
                .Count();
            long totalFileSize = filesToCopy
                .Where(file => lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime)
                .Sum(file => new FileInfo(file).Length);
            commonSave(save, totalFilesToCopy, totalFileSize, lastChangeDateTime);
        }
        catch (DirectoryNotFoundException directoryNotFound)
        {
            Console.WriteLine("The source directory path of the save is valid but does not exist. " + directoryNotFound.Message);
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
            string[] filesToCopy = Directory.GetFiles(save.sourceDirectory, "*.*", SearchOption.AllDirectories);
            int totalFilesToCopy = filesToCopy
                .Where(file => lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime)
                .Count();
            long totalFileSize = filesToCopy
                .Where(file => lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime)
                .Sum(file => new FileInfo(file).Length);
            string target = @"\" + save.name + @"\" + DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss.ff");
            DateTime startSave = DateTime.UtcNow;
            int encryptionTime = commonSaveDirectory(
                save.sourceDirectory,
                string.Concat(save.targetDirectory, target),
                save.name,
                totalFilesToCopy,
                totalFileSize,
                totalFilesToCopy,
                totalFileSize,
                save.logFileExtension,
                0,
                token,
                pauseEvent,
                progressCallback,
                lastChangeDateTime
            );
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
        catch (OperationCanceledException)
        {
            Console.WriteLine("Save operation was cancelled.");
            throw;
        }
        catch (DirectoryNotFoundException directoryNotFound)
        {
            Console.WriteLine("The source directory path of the save is valid but does not exist. " + directoryNotFound.Message);
        }
        catch (InvalidOperationException message)
        {
            throw new InvalidOperationException(message.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("The source directory path of the save is invalid or you don't have the required access. " + ex.Message);
        }
    }

    /// <inheritdoc/>
    public override (int, long) SaveDirectory(string target, int nbFilesLeftToDo, long filesSizeLeftToDo, DateTime? lastChangeDateTime = null)
    {
        string[] filesToCopy = Directory.GetFiles(target, "*.*", SearchOption.AllDirectories);
        nbFilesLeftToDo -= filesToCopy
            .Where(file => lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime)
            .Count();
        filesSizeLeftToDo -= filesToCopy
            .Where(file => lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime)
            .Sum(file => new FileInfo(file).Length);
        return (nbFilesLeftToDo, filesSizeLeftToDo);
    }
}

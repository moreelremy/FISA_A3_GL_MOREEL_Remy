using System;
using System.IO;

/// <summary>
/// Represents a backup save operation, containing details about the source, target, and strategy used.
/// </summary>
public class Save
{
    public required string name { get; set; }
    public required string sourceDirectory { get; set; }
    public required string targetDirectory { get; set; }
    public required ISaveStrategy saveStrategy { get; set; }
}

/// <summary>
/// Interface defining the contract for different save strategies.
/// </summary>
public interface ISaveStrategy
{
    /// <summary>
    /// Executes the save process using the provided save configuration.
    /// </summary>
    /// <param name="save">The save instance containing backup details.</param>
    void Save(Save save);

    /// <summary>
    /// Saves a directory by copying files and subdirectories.
    /// </summary>
    void SaveDirectory(string sourceDirectory, string targetDirectory, string saveName, int totalFilesToCopy, long totalFileSize, int nbFilesLeftToDo, long filesSizeLeftToDo, DateTime? lastChangeDateTime = null);
}

/// <summary>
/// Implements a full save strategy that copies all files and directories.
/// </summary>
public class FullSave : ISaveStrategy
{
    /// <inheritdoc/>
    public void Save(Save save)
    {
        try
        {
            string target = @"\" + save.name + @"\" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss.ff");
            // Count the number of files to copy and the total size of the files
            int totalFilesToCopy = Directory.GetFiles(save.sourceDirectory, "*.*", SearchOption.AllDirectories).Count();
            // Sum the size of all files in the source directory
            long totalFileSize = Directory.GetFiles(save.sourceDirectory, "*.*", SearchOption.AllDirectories).Sum(file => new FileInfo(file).Length);
            DateTime startSave = DateTime.UtcNow;
            SaveDirectory(save.sourceDirectory, string.Concat(save.targetDirectory,target), save.name, totalFilesToCopy, totalFileSize, totalFilesToCopy, totalFileSize);
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
                Progression: 0
            );
            DateTime endSave = DateTime.UtcNow;
            // Calculate the time taken to save
            int transferTime = (int)(endSave - startSave).TotalMilliseconds;
            Logs.GeneralLog(save, totalFileSize, transferTime);
        }
        catch (DirectoryNotFoundException directoryNotFound)
        {
            Console.WriteLine("The source directory path of the save is valid but does not exist.");
        }
        catch (Exception e)
        {
            Console.WriteLine("The source directory path of the save is invalid or you don't have the required access.");
        }
    }

    /// <inheritdoc/>
    public void SaveDirectory(string sourceDirectory, string targetDirectory, string saveName, int totalFilesToCopy, long totalFileSize, int nbFilesLeftToDo, long filesSizeLeftToDo, DateTime? lastChangeDateTime = null)
    {
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        // Copy all files in the source directory to the target directory
        foreach (string file in Directory.GetFiles(sourceDirectory))
        {
            string target = Path.Combine(targetDirectory, Path.GetFileName(file));
            File.Copy(file, target, true);
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
                filesSizeLeftToDo : filesSizeLeftToDo,
                Progression: (int)(((float)totalFileSize - (float)filesSizeLeftToDo) / (float)totalFileSize * 100)
            );
        }

        // Copy all subdirectories in the source directory to the target directory
        foreach (string directory in Directory.GetDirectories(sourceDirectory))
        {
            string target = Path.Combine(targetDirectory, Path.GetFileName(directory));
            SaveDirectory(directory, target, saveName, totalFilesToCopy, totalFileSize, nbFilesLeftToDo, filesSizeLeftToDo);
            nbFilesLeftToDo -= Directory.GetFiles(target, "*.*", SearchOption.AllDirectories).Count();
            filesSizeLeftToDo -= Directory.GetFiles(target, "*.*", SearchOption.AllDirectories).Sum(file => new FileInfo(file).Length);
        }
    }
}

/// <summary>
/// Implements a differential save strategy that only copies changed files since the last save.
/// </summary>
public class DifferentialSave : ISaveStrategy
{
    /// <inheritdoc/>
    public void Save(Save save)
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

            string target = targetRepo + @"\" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss.ff");
            // Count the number of files to copy and the total size of the files
            string[] filesToCopy = Directory.GetFiles(save.sourceDirectory, "*.*", SearchOption.AllDirectories);
            int totalFilesToCopy = filesToCopy.Where(file => lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime).Count();
            long totalFileSize = filesToCopy.Where(file => lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime).Sum(file => new FileInfo(file).Length);
            DateTime startSave = DateTime.UtcNow;
            SaveDirectory(save.sourceDirectory, target, save.name, totalFilesToCopy, totalFileSize, totalFilesToCopy, totalFileSize, lastChangeDateTime);
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
                Progression: 0
            );
            DateTime endSave = DateTime.UtcNow;
            // Calculate the time taken to save
            int transferTime = (int)(endSave - startSave).TotalMilliseconds;
            Logs.GeneralLog(save, totalFileSize, transferTime);
        }
        catch (DirectoryNotFoundException directoryNotFound)
        {
            Console.WriteLine("The source directory path of the save is valid but does not exist.");
        }
        catch (Exception e)
        {
            Console.WriteLine("The source directory path of the save is invalid or you don't have the required access.");
        }
    }

    /// <inheritdoc/>
    public void SaveDirectory(string sourceDirectory, string targetDirectory, string saveName, int totalFilesToCopy, long totalFileSize, int nbFilesLeftToDo, long filesSizeLeftToDo, DateTime? lastChangeDateTime = null)
    {
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        // Copy all files in the source directory to the target directory
        foreach (string file in Directory.GetFiles(sourceDirectory))
        {
            string target = Path.Combine(targetDirectory, Path.GetFileName(file));

            // Copy the file if it has been modified since the last save
            if (lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime)
            {
                File.Copy(file, target, true);
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
                    Progression: (int)(((float)totalFileSize - (float)filesSizeLeftToDo) / (float)totalFileSize * 100)
                );
            }
        }

        // Copy all subdirectories in the source directory to the target directory
        foreach (string directory in Directory.GetDirectories(sourceDirectory))
        {
            string target = Path.Combine(targetDirectory, Path.GetFileName(directory));
            SaveDirectory(directory, target, saveName, totalFilesToCopy, totalFileSize, nbFilesLeftToDo, filesSizeLeftToDo, lastChangeDateTime);
            // Count the number of files to copy and the total size of the files
            string[] filesToCopy = Directory.GetFiles(target, "*.*", SearchOption.AllDirectories);
            nbFilesLeftToDo -= filesToCopy.Where(file => lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime).Count();
            filesSizeLeftToDo -= filesToCopy.Where(file => lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime).Sum(file => new FileInfo(file).Length);
        }
    }
}
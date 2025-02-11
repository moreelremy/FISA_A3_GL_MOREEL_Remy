using System;
using System.IO;

public interface ISaveStrategy
{
    void Save(Save save);
    void SaveDirectory(string sourceDirectory, string targetDirectory, string saveName, int totalFilesToCopy, long totalFileSize, int nbFilesLeftToDo, long filesSizeLeftToDo, DateTime? lastChangeDateTime = null);
}

public class FullSave : ISaveStrategy
{
    public void Save(Save save)
    {
        try
        {
            if (!Directory.Exists(save.sourceDirectory))
            {
                throw new DirectoryNotFoundException();
            }
            string target = @"\" + save.name + @"\" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss.ff");
            int totalFilesToCopy = Directory.GetFiles(save.sourceDirectory, "*.*", SearchOption.AllDirectories).Count();
            long totalFileSize = Directory.GetFiles(save.sourceDirectory, "*.*", SearchOption.AllDirectories).Sum(file => new FileInfo(file).Length);
            DateTime startSave = DateTime.UtcNow;
            SaveDirectory(save.sourceDirectory, string.Concat(save.targetDirectory,target), save.name, totalFilesToCopy, totalFileSize, totalFilesToCopy, totalFileSize);
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

    public void SaveDirectory(string sourceDirectory, string targetDirectory, string saveName, int totalFilesToCopy, long totalFileSize, int nbFilesLeftToDo, long filesSizeLeftToDo, DateTime? lastChangeDateTime = null)
    {
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        foreach (string file in Directory.GetFiles(sourceDirectory))
        {
            
            string target = Path.Combine(targetDirectory, Path.GetFileName(file));
            File.Copy(file, target, true);
            long fileSize = new FileInfo(file).Length;
            nbFilesLeftToDo -= 1;
            filesSizeLeftToDo -= fileSize;
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

        foreach (string directory in Directory.GetDirectories(sourceDirectory))
        {
            string target = Path.Combine(targetDirectory, Path.GetFileName(directory));
            SaveDirectory(directory, target, saveName, totalFilesToCopy, totalFileSize, nbFilesLeftToDo, filesSizeLeftToDo);
            nbFilesLeftToDo -= Directory.GetFiles(target, "*.*", SearchOption.AllDirectories).Count();
            filesSizeLeftToDo -= Directory.GetFiles(target, "*.*", SearchOption.AllDirectories).Sum(file => new FileInfo(file).Length);
        }
    }
}

public class DifferentialSave : ISaveStrategy
{
    public void Save(Save save)
    {
        /*
        try
        {
            if (!Directory.Exists(save.sourceDirectory))
            {
                throw new DirectoryNotFoundException();
            }

            DateTime? lastChangeDateTime = null;
            string targetRepo = string.Concat(save.targetDirectory, @"\" + save.name);

            if (Directory.Exists(targetRepo))
            {
                lastChangeDateTime = Directory.GetCreationTime(targetRepo);
            }

            string target = targetRepo + @"\" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss.ff");
            DateTime startSave = DateTime.UtcNow;
            long fileSize = SaveDirectory(save.sourceDirectory, target, 0, save, lastChangeDateTime);
            DateTime endSave = DateTime.UtcNow;
            int transferTime = (int)(endSave - startSave).TotalMilliseconds;
            Logs.GeneralLog(save, fileSize, transferTime);

        }
        catch (DirectoryNotFoundException directoryNotFound)
        {
            Console.WriteLine("The source directory path of the save is valid but does not exist.");
        }
        catch (Exception e)
        {
            Console.WriteLine("The source directory path of the save is invalid or you don't have the required access.");
        }
        */
    }
    public void SaveDirectory(string sourceDirectory, string targetDirectory, string saveName, int totalFileSizetoCopy, long totalFileSize, int nbFilesLeftToDo, long filesSizeLeftToDo, DateTime? lastChangeDateTime = null)
    {
        /*
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }
        foreach (string file in Directory.GetFiles(sourceDirectory))
        {
            string target = Path.Combine(targetDirectory, Path.GetFileName(file));
            // Check if the file exists and has the same modification date
            if (lastChangeDateTime == null || File.GetLastWriteTime(file) > lastChangeDateTime)
            {
                File.Copy(file, target, true);
                fileSize += new FileInfo(file).Length;
            }
        }
        foreach (string directory in Directory.GetDirectories(sourceDirectory))
        {
            string target = Path.Combine(targetDirectory, Path.GetFileName(directory));
            SaveDirectory(directory, target, fileSize, save, lastChangeDateTime);
        }

        return fileSize;
        */
    }
}

public class Save
{
    public required string name { get; set; }
    public required string sourceDirectory { get; set; }
    public required string targetDirectory { get; set; }
    public required ISaveStrategy saveStrategy { get; set; }

    public static string ConvertToUnc(string localPath)
    {
        string fullPath = Path.GetFullPath(localPath);

        if (!Path.IsPathRooted(fullPath))
            throw new ArgumentException("Le chemin fourni n'est pas absolu.");

        string driveLetter = Path.GetPathRoot(fullPath)?.TrimEnd('\\');
        if (driveLetter == null || driveLetter.Length < 2)
            throw new ArgumentException("Impossible d'extraire la lettre du disque.");

        string uncPath = fullPath.Replace(driveLetter, $"\\\\localhost\\{driveLetter.TrimEnd(':')}$");

        return uncPath;
    }
}


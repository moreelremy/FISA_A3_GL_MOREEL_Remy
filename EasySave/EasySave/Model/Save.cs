using System;
using System.IO;

public interface ISaveStrategy
{
    void Save(Save save);

    void SaveDirectory(string sourceDirectory, string targetDirectory);
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
            string target = @"\" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss.ff_") + save.name;
            SaveDirectory(save.sourceDirectory, string.Concat(save.targetDirectory,target));
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

    public void SaveDirectory(string sourceDirectory, string targetDirectory)
    {
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        foreach (string file in Directory.GetFiles(sourceDirectory))
        {
            string target = Path.Combine(targetDirectory, Path.GetFileName(file));
            File.Copy(file, target, true);
        }

        foreach (string directory in Directory.GetDirectories(sourceDirectory))
        {
            string target = Path.Combine(targetDirectory, Path.GetFileName(directory));
            SaveDirectory(directory, target);
        }
    }
}
/*
public class incrementalsave : isavestrategy
{
    public void save(save save)
    {
        throw new notimplementedexception();
    }
}
*/

public class DifferentialSave : ISaveStrategy
{
    public void Save(Save save)
    {
        try
        {
            if (!Directory.Exists(save.sourceDirectory))
            {
                throw new DirectoryNotFoundException();
            }
            string target = @"\" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss.ff_") + save.name;
            SaveDirectory(save.sourceDirectory, string.Concat(save.targetDirectory, target));
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
    public void SaveDirectory(string sourceDirectory, string targetDirectory)
    {
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }
        foreach (string file in Directory.GetFiles(sourceDirectory))
        {
            string target = Path.Combine(targetDirectory, Path.GetFileName(file));
            // Check if the file exists and has the same modification date
            if (!File.Exists(target) || File.GetLastWriteTime(file) != File.GetLastWriteTime(target))
            {
                File.Copy(file, target, true);
            }
        }
        foreach (string directory in Directory.GetDirectories(sourceDirectory))
        {
            string target = Path.Combine(targetDirectory, Path.GetFileName(directory));
            SaveDirectory(directory, target);
        }
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


public interface ISaveStrategy
{
    void Save(Save save);
}

public class FullSave : ISaveStrategy
{
    public void Save(Save save)
    {
        throw new NotImplementedException();
    }
}
/*
public class incrementalsave : isavestrategy
{
    public void save(save save)
    {
        throw new notimplementedexception();
    }
}*/

public class DifferentialSave : ISaveStrategy
{
    public void Save(Save save)
    {
        throw new NotImplementedException();
    }
}

public class Save
{
    public required string name { get; set; }
    public required string sourceRepository { get; set; }
    public required string targetRepository { get; set; }
    public ISaveStrategy saveType { get; set; }
    public DateTime? dateSauvegarde { get; set; }

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


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
    public DateTime dateSauvegarde { get; set; }
}


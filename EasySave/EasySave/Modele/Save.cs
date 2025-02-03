public class Sauvegarde
{
    public required string name { get; set; }
    public required string sourceRepository { get; set; }
    public required string targetRepository { get; set; }
    public bool isTotal { get; set; }
    public DateTime dateSauvegarde { get; set; }


}


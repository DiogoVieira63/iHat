namespace iHat.Model;

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string ObrasCollectionName { get; set; } = null!;

    public string CapacetesCollectionName { get; set; } = null!;

    public string LogsCollectionName { get; set;} = null!;

    public string ZonasCollectionName { get; set; } = null!;

    public string MapasCollectionName  { get; set; } = null!;

}
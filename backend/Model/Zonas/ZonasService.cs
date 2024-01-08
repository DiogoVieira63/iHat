using Microsoft.Extensions.Options;
using MongoDB.Driver;
using iHat.Model.Obras;

namespace iHat.Model.Zonas;


public class ZonasService: IZonasService{

    public readonly IMongoCollection<Obra> _obraCollection;
    public readonly IMongoCollection<ZonasRisco> _zonaRiscoCollection;

    public ZonasService(IOptions<DatabaseSettings> iHatDatabaseSettings ){
        var mongoClient = new MongoClient(
            iHatDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            iHatDatabaseSettings.Value.DatabaseName);

        _obraCollection = mongoDatabase.GetCollection<Obra>(
            iHatDatabaseSettings.Value.ObrasCollectionName);

        _zonaRiscoCollection = mongoDatabase.GetCollection<ZonasRisco>(
            iHatDatabaseSettings.Value.ZonasRiscoCollectionName);
    }
    public async Task AddZonasPerigo(int idZona, List<Point> lista)
    {
        var zona = await _zonaRiscoCollection.Find(x => x.IdZona == idZona).FirstOrDefaultAsync();

        if (zona == null)
        {
            Console.WriteLine("[iHatFacade] Zona não existe.");
            return;
        }

        zona.Zonas = lista;

        try
        {
            await _zonaRiscoCollection.ReplaceOneAsync(x => x.IdZona == idZona, zona);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar a zona: {ex.Message}");
        }
        
    }

    public async Task RemoveZonasPerigo(int idZona)
    {
        var zona = await _zonaRiscoCollection.Find(x => x.IdZona == idZona).FirstOrDefaultAsync();

        if (zona == null)
        {
            Console.WriteLine("[iHatFacade] Zona não existe.");
            return;
        }

        zona.Zonas = new List<Point>();

        try
        {
            await _zonaRiscoCollection.ReplaceOneAsync(x => x.IdZona == idZona, zona);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar a zona: {ex.Message}");
        }
    }

    public async Task RemoveAllZonasPerigo(int idZona)
    {
        var zona = await _zonaRiscoCollection.Find(x => x.IdZona == idZona).FirstOrDefaultAsync();

        if (zona == null)
        {
            Console.WriteLine("[iHatFacade] Zona não existe.");
            return;
        }

        zona.Zonas = new List<Point>();

        try
        {
            await _zonaRiscoCollection.ReplaceOneAsync(x => x.IdZona == idZona, zona);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar a zona: {ex.Message}");
        }
    }

    public async Task UpdateZonasPerigo(int idZona, List<Point> lista)
    {
        var zona = await _zonaRiscoCollection.Find(x => x.IdZona == idZona).FirstOrDefaultAsync();

        if (zona == null)
        {
            Console.WriteLine("[iHatFacade] Zona não existe.");
            return;
        }

        zona.Zonas = lista;

        try
        {
            await _zonaRiscoCollection.ReplaceOneAsync(x => x.IdZona == idZona, zona);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar a zona: {ex.Message}");
        }
    }
}
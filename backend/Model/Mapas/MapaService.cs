using Microsoft.Extensions.Options;
using MongoDB.Driver;
using iHat.Model.Obras;
using iHat.Model.Zonas;

namespace iHat.Model.Mapas;


public class MapaService: IMapaService{

    public readonly IMongoCollection<Mapa> _mapaCollection;

    public MapaService(IOptions<DatabaseSettings> iHatDatabaseSettings ){
        var mongoClient = new MongoClient(
            iHatDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            iHatDatabaseSettings.Value.DatabaseName);

        _mapaCollection = mongoDatabase.GetCollection<Mapa>(
            iHatDatabaseSettings.Value.MapasCollectionName);
    }

    public async Task<string?> Add(string name, string svg){
        var mapa = new Mapa(name, svg);
        await _mapaCollection.InsertOneAsync(mapa);
        return mapa.Id;
    }


    public async Task AddZoneRiscotoMapa(string name, List<ZonasRisco> lista){
        var mapa = await _mapaCollection.Find(x => x.Name == name).FirstOrDefaultAsync();

        if (mapa == null)
        {
            Console.WriteLine("[iHatFacade] Mapa não existe.");
            return;
        }

        mapa.Zonas = lista;

        try
        {
            await _mapaCollection.ReplaceOneAsync(x => x.Name == name, mapa);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar a zona: {ex.Message}");
        }
    }

    public async Task RemoveZonasPerigotoMapa(string name){
        var mapa = await _mapaCollection.Find(x => x.Name == name).FirstOrDefaultAsync();

        if (mapa == null)
        {
            Console.WriteLine("[iHatFacade] Mapa não existe.");
            return;
        }

        mapa.Zonas = new List<ZonasRisco>();

        try
        {
            await _mapaCollection.ReplaceOneAsync(x => x.Name == name, mapa);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar a zona: {ex.Message}");
        }
    }

    public async Task RemoveAllZonasPerigotoMapa( string name){
        var mapa = await _mapaCollection.Find(x => x.Name == name).FirstOrDefaultAsync();

        if (mapa == null)
        {
            Console.WriteLine("[iHatFacade] Mapa não existe.");
            return;
        }

        mapa.Zonas = new List<ZonasRisco>();

        try
        {
            await _mapaCollection.ReplaceOneAsync(x => x.Name == name, mapa);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar a zona: {ex.Message}");
        }
    }

    public async Task UpdateZonasPerigotoMapa(string Name, List<ZonasRisco> lista){
        var mapa = await _mapaCollection.Find(x => x.Name == Name).FirstOrDefaultAsync();

        if (mapa == null)
        {
            Console.WriteLine("[iHatFacade] Mapa não existe.");
            return;
        }

        mapa.Zonas = lista;

        try
        {
            await _mapaCollection.ReplaceOneAsync(x => x.Name == Name, mapa);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar a zona: {ex.Message}");
        }
    }


    public async Task<Mapa?> GetMapaById(string id){
        var mapa = await _mapaCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return mapa;
    }
}
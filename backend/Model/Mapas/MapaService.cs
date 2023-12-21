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
        Console.WriteLine(mapa.Id);
        await _mapaCollection.InsertOneAsync(mapa);
        Console.WriteLine(mapa.Id);
        return mapa.Id;
    }


    public async Task AddZoneRiscotoMapa(string id, List<ZonasRisco> lista){
        var mapa = await _mapaCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        if (mapa == null)
        {
            Console.WriteLine("[iHatFacade] Mapa não existe.");
            return;
        }

        mapa.Zonas = lista;

        try
        {
            await _mapaCollection.ReplaceOneAsync(x => x.Id == id, mapa);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar a zona: {ex.Message}");
        }
    }

    public async Task RemoveZonasPerigotoMapa(string id){
        var mapa = await _mapaCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        if (mapa == null)
        {
            Console.WriteLine("[iHatFacade] Mapa não existe.");
            return;
        }

        mapa.Zonas = new List<ZonasRisco>();

        try
        {
            await _mapaCollection.ReplaceOneAsync(x => x.Id == id, mapa);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar a zona: {ex.Message}");
        }
    }

    public async Task RemoveAllZonasPerigotoMapa( string id){
        var mapa = await _mapaCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        if (mapa == null)
        {
            Console.WriteLine("[iHatFacade] Mapa não existe.");
            return;
        }

        mapa.Zonas = new List<ZonasRisco>();

        try
        {
            await _mapaCollection.ReplaceOneAsync(x => x.Id == id, mapa);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar a zona: {ex.Message}");
        }
    }

    public async Task UpdateZonasPerigotoMapa(string id, List<ZonasRisco> lista){
        var mapa = await _mapaCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        if (mapa == null)
        {
            Console.WriteLine("[iHatFacade] Mapa não existe.");
            return;
        }

        mapa.Zonas = lista;

        try
        {
            await _mapaCollection.ReplaceOneAsync(x => x.Id == id, mapa);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar a zona: {ex.Message}");
        }
    }


}
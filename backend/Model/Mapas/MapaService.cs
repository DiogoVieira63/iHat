using Microsoft.Extensions.Options;
using MongoDB.Driver;
using iHat.Model.Zonas;

namespace iHat.Model.Mapas;


public class MapaService: IMapaService{

    public readonly IMongoCollection<Mapa> _mapaCollection;

    private readonly ILogger<MapaService> _logger;

    public MapaService(IOptions<DatabaseSettings> iHatDatabaseSettings, ILogger<MapaService> logger ){
        var mongoClient = new MongoClient(
            iHatDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            iHatDatabaseSettings.Value.DatabaseName);

        _mapaCollection = mongoDatabase.GetCollection<Mapa>(
            iHatDatabaseSettings.Value.MapasCollectionName);

        _logger = logger;
    }

    public async Task<Mapa?> GetMapaById(string id){
        var mapa = await _mapaCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return mapa;
    }

    /*public async Task<List<ZonasRisco>?> GetZonasdeRisco(string id){
        var mapa = await _mapaCollection.Find(x => x.Id == id).FirstOrDefaultAsync() ?? throw new Exception("Mapa não encontrado.");
        return mapa.Zonas;
    }*/

    public async Task<string?> Add(string name, string svg, int floor){
        var mapa = new Mapa(name, svg, floor);
        await _mapaCollection.InsertOneAsync(mapa);
        return mapa.Id;
    }

    public async Task RemoveZonasPerigoOfMapa(string id){
        var mapa = await _mapaCollection.Find(x => x.Id == id).FirstOrDefaultAsync() ?? throw new Exception("Mapa não encontrado.");
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

    public async Task RemoveMapas(List<string> mapas){
        foreach (var id in mapas){
            await _mapaCollection.DeleteOneAsync(x => x.Id == id);
        }
    }

    public async Task UpdateZonasPerigoOfMapa(string id, List<ZonasRisco> zonas){
        var mapa = await _mapaCollection.Find(x => x.Id == id).FirstOrDefaultAsync() ?? throw new Exception("Mapa não encontrado.");
        var mapaUpdate = Builders<Mapa>.Update.Set(x => x.Zonas, zonas);
        await _mapaCollection.UpdateOneAsync(x => x.Id == id, mapaUpdate);
    }

    public async Task UpdateFloorNumber(string id, int newFloorNumber){
        var mapa = await _mapaCollection.Find(x => x.Id == id).FirstOrDefaultAsync() ?? throw new Exception("Mapa não encontrado.");
        var floorUpdate = Builders<Mapa>.Update.Set(x => x.Floor, newFloorNumber);
        await  _mapaCollection.UpdateOneAsync(x => x.Id == id, floorUpdate);
    }
}
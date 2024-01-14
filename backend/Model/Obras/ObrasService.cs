using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace iHat.Model.Obras;

public class ObrasService: IObrasService{

    public readonly IMongoCollection<Obra> _obraCollection;
    private readonly ILogger<ObrasService> _logger;

    // public readonly IMongoCollection<Mapa> _mapaCollection;

    public ObrasService(IOptions<DatabaseSettings> iHatDatabaseSettings, ILogger<ObrasService> logger){
        var mongoClient = new MongoClient(
            iHatDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            iHatDatabaseSettings.Value.DatabaseName);

        _obraCollection = mongoDatabase.GetCollection<Obra>(
            iHatDatabaseSettings.Value.ObrasCollectionName);

        /*_mapaCollection = mongoDatabase.GetCollection<Mapa>(
            iHatDatabaseSettings.Value.MapasCollectionName);*/

        _logger = logger;
    }

    public async Task<bool> CheckIfObraExists(string id){
        var obra = await _obraCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return obra != null;
    }

    public async Task<List<Obra>> GetObrasOfResponsavel(int idResponsavel){
        var obras = await _obraCollection.Find(x => x.IdResponsavel == idResponsavel).ToListAsync();
        return obras;
    }

    public async Task<Obra?> GetConstructionById(string id){
        var obras = await _obraCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return obras;
    }

    public async Task<Obra?> GetObraWithCapaceteId(int nCapaceteToFind){
        var obra = await _obraCollection.Find(o => o.Capacetes.Contains(nCapaceteToFind)).FirstOrDefaultAsync();
        return obra;
    }

    public async Task<List<int>> GetAllCapacetesOfObra(string id){
        var obra = await _obraCollection.Find(x => x.Id == id).FirstOrDefaultAsync() ?? throw new Exception("Obra não encontrada.");
        return obra.Capacetes;
    }

    public async Task<string?> AddObra(string name, int idResponsavel, List<string> mapa){
        var checkIfConstructionSameName = await _obraCollection.Find(x => x.Nome == name).FirstOrDefaultAsync();
        if(checkIfConstructionSameName != null){
            throw new Exception("Construction with this name already exists.");
        }

        var newObra = new Obra(name, idResponsavel, mapa);
        await _obraCollection.InsertOneAsync(newObra);
        return newObra.Id;
    }

    public async Task<List<string>> AddListaMapaToObra(string id, List<string> mapas){
        var obra = await _obraCollection.Find(x => x.Id == id).FirstOrDefaultAsync() ?? throw new Exception("Obra não encontrada.");
        if(!obra.CanChangeMap()){
            throw new Exception("Estado atual não permite atualizar o mapa.");
        }
        var listaPreviousMapas = obra.Mapa;
        var obraUpdate = Builders<Obra>.Update.Set(x => x.Mapa, mapas);
        await _obraCollection.UpdateOneAsync(x => x.Id == id, obraUpdate);
        return listaPreviousMapas;
    }

    public async Task AddCapaceteToObra(int idCapacete, string idObra){
        var obra = await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync() ?? throw new Exception("Obra não encontrada.");
        
        if (!obra.CanAddCapacete())
            throw new Exception("Estado atual da obra não permite adicionar um novo capacete");

        obra.Capacetes.Add(idCapacete);
        var update = Builders<Obra>.Update.Set(x => x.Capacetes, obra.Capacetes);
        await _obraCollection.UpdateOneAsync(x => x.Id == idObra, update);
    }

    public async Task RemoveObraByIdAsync(string id)
    {
        await _obraCollection.DeleteOneAsync(o => o.Id == id);
    }

    public async Task RemoveCapaceteToObra(int nCapacete, string idObra){
        var obra = await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync() ?? throw new Exception("Obra não encontrada.");
        
        if(!obra.CanAddCapacete())
            throw new Exception("Estado atual da obra não permite alterar a lista de capacetes.");
        
        obra.Capacetes.Remove(nCapacete);

        var obraUpdate = Builders<Obra>.Update.Set(x => x.Capacetes, obra.Capacetes);
        await _obraCollection.UpdateOneAsync(x => x.Id == idObra, obraUpdate);
    }

    public async Task UpdateEstadoObra(string id, string estado)
    {
        var obra = await _obraCollection.Find(x => x.Id == id).FirstOrDefaultAsync() ?? throw new Exception("Obra não encontrada.");
        
        if (!obra.CanChangeStatus())
            throw new Exception("Estado atual da obra não permite atualizar para novo estado.");

        var obraUpdate = Builders<Obra>.Update.Set(x => x.Status, estado);
        await _obraCollection.UpdateOneAsync(x => x.Id == id, obraUpdate);
    }

    public async Task UpdateNomeObra(string id, string nome)
    {
        var obra = await _obraCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (obra == null)
            throw new Exception("Obra não encontrada.");

        if(!obra.CanChangeName())
            throw new Exception("Estado atual da obra não permite que atualizar o nome.");
    
        var obraUpdate = Builders<Obra>.Update.Set(x => x.Nome, nome);
        await _obraCollection.UpdateOneAsync(x => x.Id == id, obraUpdate);
    }

    public async Task<bool> UpdateZonasRiscoObra(string idObra, string idMapa){
        var obra =  await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync() ?? throw new Exception("Obra não encontrada.");
        if(!obra.Mapa.Contains(idMapa))
            return false;
        if(!obra.CanChangeMap())
            return false;
        return true;
    }

    //rever
    /*public async Task UpdateZonasRiscoObra(string idObra, string idMapa, List<ZonasRisco> zonas){
        var obra =  await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync() ?? throw new Exception("Obra não encontrada.");
        if (obra.Mapa.Contains(idMapa)){
            var mapa = await _mapaCollection.Find(x => x.Id == idMapa).FirstOrDefaultAsync() ?? throw new Exception("Mapa não encontrada.");
            if (mapa is not null){
                var mapaFilter = Builders<Mapa>.Filter.Eq(x => x.Id, idMapa);
                var mapaUpdate = Builders<Mapa>.Update.Set(x => x.Zonas, zonas);
                await _mapaCollection.UpdateOneAsync(mapaFilter, mapaUpdate);

            }else{
                throw new Exception("Mapa não encontrado.");
            }
        }else{
            throw new Exception("Mapa não encontrado.");
        }
    }*/
    
}
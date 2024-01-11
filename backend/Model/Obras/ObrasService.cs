using Microsoft.Extensions.Options;
using MongoDB.Driver;
using iHat.Model.Zonas;
using iHat.Model.Mapas;
using Microsoft.AspNetCore.Identity;

namespace iHat.Model.Obras;

public class ObrasService: IObrasService{

    public readonly IMongoCollection<Obra> _obraCollection;
    private readonly ILogger<ObrasService> _logger;

    public readonly IMongoCollection<Mapa> _mapaCollection;

    public ObrasService(IOptions<DatabaseSettings> iHatDatabaseSettings, ILogger<ObrasService> logger){
        var mongoClient = new MongoClient(
            iHatDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            iHatDatabaseSettings.Value.DatabaseName);

        _obraCollection = mongoDatabase.GetCollection<Obra>(
            iHatDatabaseSettings.Value.ObrasCollectionName);

        _mapaCollection = mongoDatabase.GetCollection<Mapa>(
            iHatDatabaseSettings.Value.MapasCollectionName);

        _logger = logger;
    }

    public async Task<List<Obra>> GetObrasOfResponsavel(int idResponsavel){
        var obras = await _obraCollection.Find(x => x.IdResponsavel == idResponsavel).ToListAsync();
        return obras;
    }

    public async Task<Obra?> GetConstructionById(string id){
        var obras = await _obraCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return obras;
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

    public async Task RemoveObraByIdAsync(string id)
    {
        await _obraCollection.DeleteOneAsync(o => o.Id == id);
    }

    public async Task AlteraEstadoObra(string id, string estado)
    {
        var obra = await _obraCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (obra == null)
            throw new Exception("Obra não encontrada.");
        


        obra.Status = estado;

        try{
            _obraCollection.ReplaceOne(x => x.Id == id, obra);
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Erro ao atualizar a obra: {ex.Message}");
        }
    }






    public async Task UpdateNomeObra(string idObra, string nome)
    {
        var obra = await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync();

        if (obra == null)
        {
            Console.WriteLine("[iHatFacade] Obra não existe.");
            return;
        }

        obra.Nome = nome;

        try
        {
            await _obraCollection.ReplaceOneAsync(x => x.Id == idObra, obra);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar a obra: {ex.Message}");
        }
    }


    public async Task<Obra?> GetObraWithCapaceteId(int nCapaceteToFind){
        var obra = await _obraCollection.Find(o => o.Capacetes.Contains(nCapaceteToFind)).FirstOrDefaultAsync();
        return obra;
    }



    public async Task<List<int>> GetAllCapacetesOfObra(string idObra){
        var obra = await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync();
        if (obra == null){
            throw new Exception("Obra não encontrada.");
        }

        return obra.Capacetes;
    }

    public async Task<List<string>> AddListaMapaToObra(string id, List<string> mapas){
        var obra = await _obraCollection.Find(x => x.Id == id).FirstOrDefaultAsync() ?? throw new Exception("Obra não encontrada.");
        var listaPreviousMapas = obra.Mapa;
        var obraFilter = Builders<Obra>.Filter.Eq(x => x.Id, id);
        var obraUpdate = Builders<Obra>.Update.Set(x => x.Mapa, mapas);
        await _obraCollection.UpdateOneAsync(obraFilter, obraUpdate);
        return listaPreviousMapas;
    }









        /*
        public async Task DeleteCapaceteToObra(string id, string idObra){
            var capacete = await _capaceteCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (capacete != null)
            {
                if (capacete.Status == "Em uso")
                {
                    var obra = await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync();
                    if (obra != null)
                    {
                        obra.Capacetes.Remove(id);

                        // Atualize a coleção de obras
                        var obraFilter = Builders<Obra>.Filter.Eq(x => x.Id, idObra);
                        var obraUpdate = Builders<Obra>.Update.Set(x => x.Capacetes, obra.Capacetes);
                        await _obraCollection.UpdateOneAsync(obraFilter, obraUpdate);

                        // Atualize a coleção de capacetes
                        // var capaceteFilter = Builders<Capacete>.Filter.Eq(x => x.Id, id);
                        // var capaceteUpdate = Builders<Capacete>.Update.Set(x => x.Obra, null);
                        // await _capaceteCollection.UpdateOneAsync(capaceteFilter, capaceteUpdate);
                        var capaceteFilter = Builders<Capacete>.Filter.Eq(x => x.Id, id);
                        await _capaceteCollection.DeleteOneAsync(capaceteFilter);
                    }
                    else
                    {
                        throw new Exception("Obra não encontrada.");
                    }
                }
                else
                {
                    throw new Exception("Capacete não pode ser removido da obra, pois não está em uso.");
                }
            }
            else
            {
                throw new Exception("Capacete não encontrado.");
            }
        }*/

    public async Task<bool> CheckIfObraExists(string idObra){
        var obra = await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync();
        return obra != null;
    }

    public async Task AddCapaceteToObra(int idCapacete, string idObra){
        var obra = await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync();
        obra.Capacetes.Add(idCapacete);
        
        var filter = Builders<Obra>.Filter.Eq(x => x.Id, idObra);
        var update = Builders<Obra>.Update.Set(x => x.Capacetes, obra.Capacetes);
        await _obraCollection.UpdateOneAsync(filter, update);
    }

    public async Task DeleteCapaceteToObra(int nCapacete, string idObra){
        
        var obra = await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync() ?? throw new Exception("Obra não encontrada.");
        obra.Capacetes.Remove(nCapacete);

        var obraFilter = Builders<Obra>.Filter.Eq(x => x.Id, idObra);
        var obraUpdate = Builders<Obra>.Update.Set(x => x.Capacetes, obra.Capacetes);
        await _obraCollection.UpdateOneAsync(obraFilter, obraUpdate);
    
    }

    //rever
    public async Task UpdateZonasRiscoObra(string idObra, string idMapa, List<ZonasRisco> zonas){
        var obra =  await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync() ?? throw new Exception("Obra não encontrada.");
        if (obra.Mapa.Contains(idMapa)){
            var mapa = await _mapaCollection.Find(x => x.Id == idMapa).FirstOrDefaultAsync() ?? throw new Exception("Mapa não encontrada.");
            if (mapa is not null){
                var mapaFilter = Builders<Mapa>.Filter.Eq(x => x.Id, idMapa);
                var mapaUpdate = Builders<Mapa>.Update.Set(x => x.Zonas, zonas);
                await _mapaCollection.UpdateOneAsync(mapaFilter, mapaUpdate);

                // var obraFilter = Builders<Obra>.Filter.Eq(x => x.Id, idObra);
                // var obraUpdate = Builders<Obra>.Update.Set(x => x.Mapa, obra.Mapa);
                // await _obraCollection.UpdateOneAsync(obraFilter, obraUpdate);
                }else{
                    throw new Exception("Mapa não encontrado.");
                }
        }else{
            throw new Exception("Mapa não encontrado.");
        }
    }  
    
}
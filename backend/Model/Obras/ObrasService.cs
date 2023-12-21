using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace iHat.Model.Obras;

public class ObrasService: IObrasService{

    public readonly IMongoCollection<Obra> _obraCollection;
    private readonly ILogger<ObrasService> _logger;


    public ObrasService(IOptions<DatabaseSettings> iHatDatabaseSettings, ILogger<ObrasService> logger){
        var mongoClient = new MongoClient(
            iHatDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            iHatDatabaseSettings.Value.DatabaseName);

        _obraCollection = mongoDatabase.GetCollection<Obra>(
            iHatDatabaseSettings.Value.ObrasCollectionName);

        _logger = logger;
    }


    public async Task<List<Obra>> GetObrasOfResponsavel(int idResponsavel){
        var obras = await _obraCollection.Find(x => x.IdResponsavel == idResponsavel).ToListAsync();

        if (obras.Count == 0)
        {
            // Nenhum resultado encontrado, retornar uma lista vazia ou fazer outra ação apropriada.
            return new List<Obra>();
        }
        return obras;
    }

    public async Task AddObra(string name, int idResponsavel, List<string> mapa){

        /*if (status != "Planeada"){
            _logger.LogInformation("Status of the new Construction is different from \"Planeada\".");
        }*/

        var newObra = new Obra(name, idResponsavel, mapa);

        var checkIfConstructionSameName = 
            await _obraCollection.Find(x => x.Name == name).FirstOrDefaultAsync();

        if(checkIfConstructionSameName != null){
            throw new Exception("Construction with this name already exists.");
        }

        try{
            await _obraCollection.InsertOneAsync(newObra);
        }
        catch (Exception e){
            throw new Exception(e.Message);
        }
    }

    public async Task<Obra> GetConstructionById(string idObra){
        var obras = await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync();
        return obras;
    }





    public async Task RemoveObraByIdAsync(string obraId)
    {
        var filter = Builders<Obra>.Filter.Eq(o => o.Id, obraId);

        await _obraCollection.DeleteOneAsync(filter);
    }


    public void AlteraEstadoObra(string id, string estado)
    {
        var obra = _obraCollection.Find(x => x.Id == id).FirstOrDefault();

        if (obra == null)
        {
            Console.WriteLine("[iHatFacade] Obra não existe.");
            return;
        }   

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

        obra.Name = nome;

        try
        {
            await _obraCollection.ReplaceOneAsync(x => x.Id == idObra, obra);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar a obra: {ex.Message}");
        }
    }


    public async Task<string?> GetIdObraWithCapaceteId(int nCapaceteToFind){
        var obra = await _obraCollection.Find(o => o.Capacetes.Contains(nCapaceteToFind)).FirstOrDefaultAsync();
        return obra == null ? null : obra.Id;
    }



    public async Task<List<int>> GetAllCapacetesOfObra(string idObra){
        var obra = await _obraCollection.Find(x => x.Id == idObra).FirstOrDefaultAsync();
        if (obra == null){
            throw new Exception("Obra não encontrada.");
        }

        return obra.Capacetes;
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
}
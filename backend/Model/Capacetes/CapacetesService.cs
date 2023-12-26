using System.Collections;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace iHat.Model.Capacetes;

public class CapacetesService: ICapacetesService{

    public readonly IMongoCollection<Capacete> _capaceteCollection;

    public CapacetesService(IOptions<DatabaseSettings> iHatDatabaseSettings){
        var mongoClient = new MongoClient(
            iHatDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            iHatDatabaseSettings.Value.DatabaseName);

        _capaceteCollection = mongoDatabase.GetCollection<Capacete>(
            iHatDatabaseSettings.Value.CapacetesCollectionName);
    }

    public async Task<List<Capacete>> GetAll(){
        return await _capaceteCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Capacete?> GetById(int nCapacete){
        return await _capaceteCollection.Find(x => x.NCapacete == nCapacete).FirstOrDefaultAsync();
    }

    public async Task Add(int nCapacete){
        var c = await _capaceteCollection.Find(x => x.NCapacete == nCapacete).FirstOrDefaultAsync();
        if(c != null){
            throw new Exception("Capacete já existe na base de dados.");
        }

        var capacete = new Capacete( nCapacete, Capacete.Livre , null);
        await _capaceteCollection.InsertOneAsync(capacete);
    }

    public async Task<List<Capacete>> GetAllHelmetsFromList(List<int> listNCapacetes){
        var listaCapacete = new List<Capacete>();
        foreach (var c in listNCapacetes){
            var capacete = await _capaceteCollection.Find(x => x.NCapacete == c).FirstOrDefaultAsync();
            if(capacete != null){
                listaCapacete.Add(capacete);
            }
        }
        return listaCapacete;
    }

    public async Task<bool> CheckIfCapaceteExists(int nCapacete){
        var capacete = await _capaceteCollection.Find(x => x.NCapacete == nCapacete).FirstOrDefaultAsync();
        return capacete != null;
    }

        public async Task DeleteCapaceteToObra(string id, string idObra){
            var capacete = await _capaceteCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (capacete != null)
            {
                if (capacete.Status == "Associado à obra")
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
        }

    public async Task<bool> CheckIfHelmetIfBeingUsed(int nCapacete){
        var capacete = await _capaceteCollection.Find(x => x.NCapacete == nCapacete).FirstOrDefaultAsync();
        if (capacete == null)
            return false;
        return capacete.Status == "Em Uso";
    }

    public async Task UpdateCapaceteStatus(int nCapacete, string status){
        var capacete = await _capaceteCollection.Find(x => x.NCapacete == nCapacete).FirstOrDefaultAsync();
        if(capacete == null)
            throw new Exception("Capacete não encontrado.");

        capacete.Status = status;
        var capaceteFilter = Builders<Capacete>.Filter.Eq(x => x.NCapacete, nCapacete);
        var capaceteUpdate = Builders<Capacete>.Update.Set(x => x.Status, capacete.Status);
        await _capaceteCollection.UpdateOneAsync(capaceteFilter, capaceteUpdate);
    }

    public async Task AddCapaceteToObra(int nCapacete){
        await UpdateCapaceteStatus(nCapacete, "Associado a obra");
    }

    public async Task UpdateCapaceteStatusToLivre(int nCapacete){
        await UpdateCapaceteStatus(nCapacete, "Livre");
    }
}

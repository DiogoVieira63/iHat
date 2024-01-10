using System.Collections;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace iHat.Model.Capacetes;

public class CapacetesService: ICapacetesService{

    public readonly IMongoCollection<Capacete> _capaceteCollection;

    private readonly ILogger<CapacetesService> _logger;

    public CapacetesService(IOptions<DatabaseSettings> iHatDatabaseSettings, ILogger<CapacetesService> logger ){
        var mongoClient = new MongoClient(
            iHatDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            iHatDatabaseSettings.Value.DatabaseName);

        _capaceteCollection = mongoDatabase.GetCollection<Capacete>(
            iHatDatabaseSettings.Value.CapacetesCollectionName);

        _logger = logger;
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

    public async Task<bool> CheckIfHelmetIfBeingUsed(int nCapacete){
        var capacete = await _capaceteCollection.Find(x => x.NCapacete == nCapacete).FirstOrDefaultAsync();
        if (capacete == null)
            return false;
        // return capacete.Status == "Em Uso";
        return capacete.Status == "Associado à Obra";
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
        await UpdateCapaceteStatus(nCapacete, "Associado à Obra");
    }

    public async Task UpdateCapaceteStatusToLivre(int nCapacete){
        await UpdateCapaceteStatus(nCapacete, "Livre");
    }

    public async Task<List<Capacete>> GetFreeHelmets(){
        return await _capaceteCollection.Find(x => x.Status == "Livre").ToListAsync();
    }

    public async Task AssociarTrabalhadorCapacete(int nCapacete, string idTrabalhador){
        var capacete = await _capaceteCollection.Find(x => x.NCapacete == nCapacete).FirstOrDefaultAsync() ?? throw new Exception("Capacete "+nCapacete+" não encontrado");
        if(capacete.Trabalhador != null){
            throw new Exception("Capacete "+nCapacete+" já tem um trabalhador associado");
        }

        var capaceteUpdate = Builders<Capacete>.Update.Set(x => x.Trabalhador, idTrabalhador);
        await _capaceteCollection.UpdateOneAsync(x => x.NCapacete == nCapacete, capaceteUpdate);
    }

    public async Task DesassociarTrabalhadorCapacete(int nCapacete, string idTrabalhador){
        var capacete = await _capaceteCollection.Find(x => x.NCapacete == nCapacete).FirstOrDefaultAsync() ?? throw new Exception("Capacete "+nCapacete+" não encontrado");
        
        /*if(capacete.Trabalhador != null){
            throw new Exception("Capacete "+nCapacete+" já tem um trabalhador associado");
        }*/

        if(capacete.Trabalhador != null && capacete.Trabalhador != idTrabalhador){
            throw new Exception("Capacete "+nCapacete+" não está associado ao trabalhador indicado.");
        }
        
        var capaceteUpdate = Builders<Capacete>.Update.Set(x => x.Trabalhador, null);
        await _capaceteCollection.UpdateOneAsync(x => x.NCapacete == nCapacete, capaceteUpdate);
    }
}

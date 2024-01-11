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
        return await _capaceteCollection.Find(x => x.Numero == nCapacete).FirstOrDefaultAsync();
    }

    public async Task<List<Capacete>> GetAllHelmetsFromList(List<int> listNumeros){
        var listaCapacete = new List<Capacete>();
        foreach (var cap in listNumeros){
            var capacete = await _capaceteCollection.Find(x => x.Numero == cap).FirstOrDefaultAsync();
            if(capacete != null){
                listaCapacete.Add(capacete);
            }
        }
        return listaCapacete;
    }

    public async Task<List<Capacete>> GetFreeHelmets(){
        return await _capaceteCollection.Find(x => x.Status == Capacete.Livre).ToListAsync();
    }


    public async Task<bool> CheckIfCapaceteExists(int nCapacete){
        var capacete = await _capaceteCollection.Find(x => x.Numero == nCapacete).FirstOrDefaultAsync();
        return capacete != null;
    }

    public async Task Add(int nCapacete){
        bool found = await CheckIfCapaceteExists(nCapacete);
        if(found) throw new Exception("Capacete já existe na base de dados.");
        
        var capacete = new Capacete(nCapacete );
        await _capaceteCollection.InsertOneAsync(capacete);
    }


    public async Task<string> CheckIfHelmetIfBeingUsed(int nCapacete){
        var capacete = await _capaceteCollection.Find(x => x.Numero == nCapacete).FirstOrDefaultAsync();
        if (capacete == null)
            return ;
        // return capacete.Status == "Em Uso";
        return ;
    }

    public async Task UpdateCapaceteStatus(int nCapacete, string status){
        var capacete = await _capaceteCollection.Find(x => x.Numero == nCapacete).FirstOrDefaultAsync();
        if(capacete == null) throw new Exception("Capacete não encontrado.");

        var capaceteUpdate = Builders<Capacete>.Update.Set(x => x.Status, status);
        await _capaceteCollection.UpdateOneAsync(x => x.Numero == nCapacete, capaceteUpdate);
    }

    public async Task AddCapaceteToObra(int nCapacete){
        await UpdateCapaceteStatus(nCapacete, );
    }

    public async Task UpdateCapaceteStatusToLivre(int nCapacete){
        await UpdateCapaceteStatus(nCapacete, Capacete.Livre);
    }


    public async Task AssociarTrabalhadorCapacete(int nCapacete, string idTrabalhador){
        var capacete = await _capaceteCollection.Find(x => x.Numero == nCapacete).FirstOrDefaultAsync() 
            ?? throw new Exception("Capacete "+nCapacete+" não encontrado");
        if(!capacete.CanAddTrabalhador()) 
            throw new Exception("Capacete "+nCapacete+" já tem um trabalhador associado");
        var capaceteUpdate = Builders<Capacete>.Update.Set(x => x.Trabalhador, idTrabalhador);
        await _capaceteCollection.UpdateOneAsync(x => x.Numero == nCapacete, capaceteUpdate);

        await UpdateCapaceteStatus(nCapacete, Capacete.EmUso);
    }

    public async Task DesassociarTrabalhadorCapacete(int nCapacete, string idTrabalhador){
        var capacete = await _capaceteCollection.Find(x => x.Numero == nCapacete).FirstOrDefaultAsync() 
            ?? throw new Exception("Capacete "+nCapacete+" não encontrado");
        if(capacete.CanRemoveTrabalhador(idTrabalhador))
            throw new Exception("Capacete "+nCapacete+" não está associado ao trabalhador indicado.");
        
        var capaceteUpdate = Builders<Capacete>.Update.Set(x => x.Trabalhador, null);
        await _capaceteCollection.UpdateOneAsync(x => x.Numero == nCapacete, capaceteUpdate);

        await UpdateCapaceteStatus(nCapacete, Capacete.Livre);
    }
}

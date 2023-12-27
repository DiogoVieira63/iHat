using System.Collections;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace iHat.Model.MensagensCapacete;
public class MensagemCapaceteService {

    public readonly IMongoCollection<MensagemCapacete> _mensagemcapaceteCollection;

    public MensagemCapaceteService(IOptions<DatabaseSettings> iHatDatabaseSettings){
        var mongoClient = new MongoClient(
            iHatDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            iHatDatabaseSettings.Value.DatabaseName);

        _mensagemcapaceteCollection = mongoDatabase.GetCollection<MensagemCapacete>(
            iHatDatabaseSettings.Value.MensagensCapaceteCollectionName);
    }


    public async Task Add(MensagemCapacete mensagem){
        await _mensagemcapaceteCollection.InsertOneAsync(mensagem);
    }

    public MensagemCapacete GetUltimosDadosDoCapacete(int nCapacete){
        var sortDefinition = Builders<MensagemCapacete>.Sort.Descending("timestamp");

        var listaMensagemCapacete = _mensagemcapaceteCollection.Find(x => x.NCapacete == nCapacete).Sort(sortDefinition).FirstOrDefault();

        return listaMensagemCapacete;
    }


}

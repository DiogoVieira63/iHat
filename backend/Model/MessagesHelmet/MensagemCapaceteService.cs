using System.Collections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SignalR.Hubs;
using System.Linq;
using iHat.MensagensCapacete.Values;

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

    public async Task<List<MensagemCapacete>> GetUltimosDadosDoCapacete(int nCapacete){
        var sortDefinition = Builders<MensagemCapacete>.Sort.Descending("timestamp");
        var listaMensagemCapacete = await _mensagemcapaceteCollection.Find(x => x.NCapacete == nCapacete).Sort(sortDefinition).ToListAsync();
        var moreRecentMensagens = listaMensagemCapacete.Take(20).ToList();
        return moreRecentMensagens;
    }

    public async Task<Location?> GetLastLocation(int nCapacete){
        var sortDefinition = Builders<MensagemCapacete>.Sort.Descending("timestamp");
        var mostRecentMessage = await _mensagemcapaceteCollection.Find(x => x.NCapacete == nCapacete).Sort(sortDefinition).FirstOrDefaultAsync();
        return mostRecentMessage == null ? null : mostRecentMessage.Location;
    }
}

using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace iHat.Model.Logs;

public class LogsService: ILogsService{

    public readonly IMongoCollection<Log> _logsCollection;

    public LogsService(IOptions<DatabaseSettings> iHatDatabaseSettings){
        var mongoClient = new MongoClient(
            iHatDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            iHatDatabaseSettings.Value.DatabaseName);

        _logsCollection = mongoDatabase.GetCollection<Log>(
            iHatDatabaseSettings.Value.LogsCollectionName);
    }

    public async Task<List<Log>> GetLogsOfObra(string idObra){
        return await _logsCollection.Find(x => x.IdObra == idObra).ToListAsync();
    }

    public async Task Add(Log log){
        await _logsCollection.InsertOneAsync(log);
    }
}
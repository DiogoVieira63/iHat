using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SignalR.Hubs;

namespace iHat.Model.Logs;

public class LogsService: ILogsService{

    public readonly IMongoCollection<Log> _logsCollection;

    private readonly ILogger<LogsService> _logger;
    
    public LogsService(IOptions<DatabaseSettings> iHatDatabaseSettings, ILogger<LogsService> logger){
        var mongoClient = new MongoClient(
            iHatDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            iHatDatabaseSettings.Value.DatabaseName);

        _logsCollection = mongoDatabase.GetCollection<Log>(
            iHatDatabaseSettings.Value.LogsCollectionName);

        _logger = logger;
    }

    public async Task<List<Log>> GetLogsOfObra(string idObra){
        return await _logsCollection.Find(x => x.IdObra == idObra && x.Timestamp == DateTime.Today).ToListAsync();
    }

    public async Task Add(Log log){
        await _logsCollection.InsertOneAsync(log);
    }

    public async Task<List<Log>> GetLogsOfObraByDate(string idObra, DateTime date){
        return await _logsCollection.Find(x => x.IdObra == idObra && x.Timestamp == date.Date).ToListAsync();
    }
}
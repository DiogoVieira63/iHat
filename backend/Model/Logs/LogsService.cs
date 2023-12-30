using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SignalR.Hubs;

namespace iHat.Model.Logs;

public class LogsService: ILogsService{

    public readonly IMongoCollection<Log> _logsCollection;
    private IHubContext<LogsHub> _logsHub;

    public LogsService(IOptions<DatabaseSettings> iHatDatabaseSettings, IHubContext<LogsHub> logsHub){
        var mongoClient = new MongoClient(
            iHatDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            iHatDatabaseSettings.Value.DatabaseName);

        _logsCollection = mongoDatabase.GetCollection<Log>(
            iHatDatabaseSettings.Value.LogsCollectionName);

        _logsHub = logsHub;
    }

    public async Task<List<Log>> GetLogsOfObra(string idObra){
        return await _logsCollection.Find(x => x.IdObra == idObra).ToListAsync();
    }

    public async Task Add(Log log){
        await _logsCollection.InsertOneAsync(log);
    }

    public async Task AddAndNotifyClients(Log log){
        
        await Add(log);
        if(log.IdObra != null){
            var listaLogs = await GetLogsOfObra(log.IdObra);
            await _logsHub.Clients.Group(log.IdObra).SendAsync("UpdateLogs", listaLogs);
            Console.WriteLine("Clients Where Notified");
        }
    }
}
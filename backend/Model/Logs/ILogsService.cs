namespace iHat.Model.Logs;

public interface ILogsService{

    Task<List<Log>> GetLogsOfObra(string idObra);
    
    Task<List<Log>> GetLogsOfObraByDate(string idObra, DateTime date);

    Task<List<Log>> GetDailyLogsCapacete(string idObra, int Capacete);

    Task Add(Log log);

}
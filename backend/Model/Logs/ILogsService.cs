namespace iHat.Model.Logs;

public interface ILogsService{
    Task<List<Log>> GetLogsOfObra(string idObra);
    Task Add(Log log);
}
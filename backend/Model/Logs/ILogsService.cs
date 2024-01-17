namespace iHat.Model.Logs;

public interface ILogsService{

    /*
    Função que permite obter todos os logs de uma obra "idObra"
    Returns: A lista de Logs
    */
    Task<List<Log>> GetLogsOfObra(string idObra);

    /*
    Função que permite obter todos os logs de uma obra "idObra" e de um dia "date".
    Returns: A lista de Logs
    */
    Task<List<Log>> GetLogsOfObraByDate(string idObra, DateTime date);

    /*
    Função que devolve os logs do Capacete {Capacete} da Obra {idObra} do dia em que a função é chamada
    Returns: A lista de Logs
    */
    Task<List<Log>> GetDailyLogsCapacete(string idObra, int Capacete);

    /*
    Função que permite adicionar um novo Log
    */
    Task Add(Log log);

    /*
    Função que atualiza o valor de Vista do Log {logId} para True.
    Exceção: Se não encontrar o Log {logId}
    */
    Task MarkLogAsSeen(string logId);
}
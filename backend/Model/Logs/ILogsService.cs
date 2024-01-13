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
    Função que permite adicionar um novo Log
    */
    Task Add(Log log);
}
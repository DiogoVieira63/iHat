using iHat.MensagensCapacete.Values;
using iHat.Model.Capacetes;
using iHat.Model.Logs;
using iHat.Model.Mapas;
using iHat.Model.MensagensCapacete;
using iHat.Model.Obras;
using iHat.Model.Zonas;

public interface IiHatFacade{

    // Obras
    Task<string?> NewConstruction(string name, IFormFile? mapa, int idResponsavel);

    /*
    Função que permite obter todas as obras geridas pelo {idResponsavel}
    Returns: uma lista de obras ou uma lista vazia
    */
    Task<List<Obra>?> GetObras(int idResponsavel);    

    /*
    Função que permite obter a obra identificada por {id}
    Returns: a Obra identificada por {id} ou null se não encontrar a obra
    */
    Task<Obra?> GetConstructionById(string id);

    /*
    Função que permite obter a lista de todos os Capacetes da obra {idObra}
    Returns: A lista de Capacetes da obra.
    Levanta uma exceção se não encontrar a Obra {idObra}
    */
    Task<List<Capacete>> GetAllCapacetesdaObra(string idObra);

    /*
    Função que permite remover/apagar a Obra {obraId}
    */
    Task RemoveObraById(string obraId);

    /*
    Função que remove o capacete {nCapacete} da Obra {idObra}.
    Para tal, remove o capacate da lista da capacetes da obra e remove o idObra do capacete
    //TODO
    */
    Task RemoveCapaceteFromObra(int nCapacete, string idObra);


    Task AddCapaceteToObra(int nCapacete, string idObra);

    /*
    Função que permite atualizar o estado da Obra {id} para {estado}
    Se o novo estado {estado} for "Finalizada" ou "Cancelada", liberta todos os capacetes da obra.
    Exceção: Se não encontrar a obra {id}
    Exceção: se o estado atual da Obra for "Finalizada" ou "Cancelada"
    */
    Task UpdateEstadoObra(string id, string estado);

    /*
    Função que permite atualizar o nome da Obra {idObra}
    Exceção: se não encontrar uma obra {idObra} 
    Exceção: se o estado da obra for "Finalizada" ou "Cancelada"
    */
    Task UpdateNomeObra(string idObra, string nome);


    // Capacetes

    /*
    Função que permite obter todos os capacetes presentes no sistema.
    Returns: uma lista de Capacetes
    */
    Task<List<Capacete>> GetAllCapacetes();

    /*
    Função que permite obter o capacete com o número de Capacete {nCapacete}
    Returns: O capacete com {nCapacete} ou nulo
    */
    Task<Capacete?> GetCapacete(int nCapacete);

    /*
    Função que permite obter uma lista de todos os capacetes livres do sistema
    Returns: Uma lista dos Capacetes com Status Livre.
    */
    Task<List<Capacete>> GetFreeHelmets();

    Task UpdateCapaceteStatus(int nCapacete, string newStatus);

    /*
    Função que permite adicionar um novo Capacete com o número "nCapacete".
    O estado inicial deste capacete será "Livre"  e não estará associado a nenhum trabalhador e a nenhuma obra
    Exception: Devolbe uma exceção se um Capacete com o mesmo número "nCapacete" já existir.
    */
    Task AddCapacete(int nCapacete );

    Task<List<MensagemCapacete>?> GetUltimosDadosDoCapacete(int nCapacete);

    Task<Dictionary<int, Location>> GetLastLocationCapacetesObra(string obraId);


    // Mapa

    /*
    Função que permite obter uma lista de Mapas a partir da lista dos ids {listaMapasIds}
    Returns: Uma lista com os Mapas
    */
    Task<List<Mapa>> GetMapasFromList(List<string> listaMapasIds);

    /*
    Função que permite atualizar as zonas de risco de um mapa {idMapa} da Obra {idObra}
    Exceção: A obra não permite que as zonas de risco sejam atualizadas.
    Exceção: O mapa {idMapa} não é encontrado. 
    */
    Task UpdateZonasRiscoObra(string idObra, string idMapa, List<ZonasRisco> zonas);

    
    Task AddMapa(string idObra, IFormFile mapaFile);

    Task UpdateMapaFloorNumber(string idObra, Dictionary<string, int> newFloors);


    // Logs

    /*
    Função que permite obter todos os logs da Obra {idObra}
    Returns: Uma lista de Logs
    */
    Task<List<Log>> GetLogs(string idObra);

    /*
    Função que permite obter todos os logs da Obra {idObra} de um dia {date}
    Returns: Lista de Logs
    */
    Task<List<Log>> GetLogsByDate(string idObra, DateTime date);

    /*
    Função que permite adicionar um novo log
    */
    Task AddLogs(Log log);

}
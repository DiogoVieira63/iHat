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
    Função que permite remover/apagar a Obra {obraId}.
    Liberta também todos os capacetes (remove a obra do Capacete & o trabalhador do Capacete)
    */
    Task RemoveObraById(string obraId);

    /*
    Função que remove o capacete {nCapacete} da Obra {idObra}.
    Para tal, remove o capacate da lista da capacetes da obra e remove o idObra do capacete.
    Exceção: Se o Capacete não estiver associado à Obra
    Exceção: Se o Capacete estiver a ser utilizado por um Trabalhador
    */
    Task RemoveCapaceteFromObra(int nCapacete, string idObra);

    /*
    Função que permite adicionar um Capacete {nCapacete} à Obra {idObra}.
    Exceção: Se não encontrar a Obra
    Exceção: Se o estado do Capacete não for "Livre" e se o valor da "Obra" não for null
    Exceção: Se o estado atual da Obra não permitir adicionar um Capacete (ex: Finalizada ou Cancelada)
    */
    Task AddCapaceteToObra(int nCapacete, string idObra);

    /*
    Função que permite atualizar o estado da Obra {id} para {estado}
    Se o novo estado {estado} for "Finalizada" ou "Cancelada",
        remove todos os capacetes da Obr, ie, coloca o parametro Obra do Capacete a null.
    Exceção: Se não encontrar a obra {id}
    Exceção: se o estado atual da Obra for "Finalizada" ou "Cancelada"
    Exceção: se o Capacete a remover da obra não estiver associado à obra indicada
    */
    Task UpdateEstadoObra(string id, string estado);

    /*
    Função que permite atualizar o nome da Obra {idObra}
    Exceção: se não encontrar uma obra {idObra} 
    Exceção: se o estado da obra for "Finalizada" ou "Cancelada"
    */
    Task UpdateNomeObra(string idObra, string nome);   

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

    /*
    Função que permite atualizar o Status do Capacete para Não Operacional, ou de Não Operacional para Livre.
    Esta função permite que o estado do Capacete seja atualizado para não operacional a partir de qualquer estado.
    Permite também atualizar de Não Operacional para Livre
    Exceção: Se não encontrar o Capacete {nCapacete}
    */
    Task UpdateCapaceteStatusFromToNaoOperacional(int nCapacete, string newStatus);

    /*
    Função que permite adicionar um novo Capacete com o número "nCapacete".
    O estado inicial deste capacete será "Livre"  e não estará associado a nenhum trabalhador e a nenhuma obra
    Exception: Devolbe uma exceção se um Capacete com o mesmo número "nCapacete" já existir.
    */
    Task AddCapacete(int nCapacete );

    /*
    Função que permite obter as últimas 20 mensagens recebidas do Capacete {nCapacete}
    Returns: Uma lista de MensagensCapacete
    */
    Task<List<MensagemCapacete>?> GetUltimosDadosDoCapacete(int nCapacete);

    /*
    Função que permite obter as últimas localizações de todos os capacetes da Obra {obraId}
    Returns: Um Dicionario (nCapacete, LastLocation)
    */
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

    /*
    Função que permite alterar/ atualizar o mapa de obra.
    Para tal, faz um pedido HTTP para um serviço python que recebe o ficheiro {mapaFile} e divide o ficheiro IFC em SVG
    Remove também os mapas antigos da obra
    Exceção: Se o estado atual da obra não permitir atualizar os mapas
    */
    Task AddMapa(string idObra, IFormFile mapaFile);

    /*
    Função que permite atualizar o valor "Floor" de cada Mapa da Obra {idObra}.
    O parametro {newFloors} deverá ser um Dicionário que associa a cada id de Mapa (newFloors.Key) um novo "Floor" (newFloors.Value)
    Exceção: Se não encontrar uma obra {idObra}
    Exceção: Se houver algum valor repetido nos Values do newFloor
    Exceção: Se algum dos Values do newFloor for < 0 ou se houver algum número de piso em falta
    */
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

    Task<List<Log>> GetDailyLogsCapacete(string idobra, int nCapacete);

    Task MarkLogAsSeen(string id);

}
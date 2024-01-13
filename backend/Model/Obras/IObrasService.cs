using iHat.Model.Zonas;

namespace iHat.Model.Obras;

public interface IObrasService{

    /*
    Função que verifica se uma obra "id" existe
    Retorna True se a obra existir
    */
    Task<bool> CheckIfObraExists(string id);

    /*
    Função que permite obter todas as obras de um responsável com id igual a idResponsavel
    */
    Task<List<Obra>> GetObrasOfResponsavel(int idResponsavel);

    /*
    Função que permite obter uma obra com o Id igua a idObra
    Retorna a Obra com o id indicado ou null.
    */
    Task<Obra?> GetConstructionById(string id);

    /*
    Função que retorna a obra que possui o capacete nCapaceteToFind
    Retorna a Obra ou null
    */
    Task<Obra?> GetObraWithCapaceteId(int nCapaceteToFind);

    /*
    Função que retorna uma lista dos números de todos os capacetes da obra "id"
    Levanta uma exceção se não encontrar a obra com "id"
    */
    Task<List<int>> GetAllCapacetesOfObra(string id);

    /*
    Função que permite adicionar uma obra
    Levanta uma exceção se uma obra com o mesmo nome já existir
    */
    Task<string?> AddObra(string name, int idResponsavel, List<string> mapa);

    /*
    Função que permite adicionar a lista dos ids dos mapas à obra "id".
    Levanta uma exceção se a obra não for encontrada
    Levanta uma exceção se o estado atual da obra não permitir atualizar a lista de mapas
    */
    Task<List<string>> AddListaMapaToObra(string id, List<string> mapas);

    /*
    Função que permite adicionar o id do Capacete à lista de capacetes da obra.
    Levanta uma exceção se a obra não for encontrada
    Levanta uma exceção se o estado atual da obra não permitir adicionar um capacete
    */
    Task AddCapaceteToObra(int idCapacete, string idObra);

    /*
    Função que permite remover a obra com o id indicado
    */
    Task RemoveObraByIdAsync(string obraId);

    /*
    Função que permite remover um numero de capacete da lista de capacetes da obra "idObra"
    Levanta uma exceção se a obra não for encontrada
    Levanta uma exceção se o estado atual da obra não permitir remover um capacete da lista
    */
    Task RemoveCapaceteToObra(int nCapacete, string idObra);

    /*
    Função que permite atualizar o estado de uma obra.
    Levanta uma exceção se a obra "id" não for encontrada.
    Levanta uma exceção se o estado da obra for "Finalizada" ou "EmCurso".
    */
    Task UpdateEstadoObra(string id, string estado);

    /*
    Função que permite atualizar o nome de uma obra.
    Levanta uma exceção se a obra "id" não for encontrada.
    Levanta uma exceção se o estado da obra for "Finalizada" ou "EmCurso".
    */
    Task UpdateNomeObra(string idObra, string nome);


    
    
    Task UpdateZonasRiscoObra(string idObra, string idMapa, List<ZonasRisco> zonas);
}
    
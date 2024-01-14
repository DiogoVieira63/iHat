namespace iHat.Model.Capacetes;

public interface ICapacetesService{

    /**
    Função que permite obter todos os capacetes do sistema.
    Returns: uma lista dos Capacetes
    */
    Task<List<Capacete>> GetAll();

    /**
    Função que permite obter o capacete com o número de Capacete "nCapacete" indicado
    Returns: Devolve o Capacete ou nulo
    */
    Task<Capacete?> GetById(int nCapacete);

    /*
    Função que permite obter todo os Capacetes de uma lista de números de capacetes
    Returns: Uma lista com os capacetes que encontrou
    */
    Task<List<Capacete>> GetAllHelmetsFromList(List<int> listNCapacetes);

    /*
    Função que permite obter uma lista de todos os capacetes livres.
    Returns: Uma lista dos capacetes com Status Livre.
    */
    Task<List<Capacete>> GetFreeHelmets();

    /*
    Função que verifica se existe um capacete com o numero "nCapacate"
    Returns: true se encontrou o capacete
    */
    Task<bool> CheckIfCapaceteExists(int nCapacete);

    /*
    Função que permite consultar se um capacete está a ser utilizado.
    i.e. verifica se o capacete está no estado "EmUso" e se tem um trabahador associado
    Exception: se não encontrar o Capacete.
    */
    Task<bool> CheckIfCapaceteIsBeingUsed(int nCapacete);

    /**
    * Função que permite adicionar um novo Capacete com o número "nCapacete" ao sistema.
    * O estado inicial deste capacete será "Livre" e não estará associado a nenhum trabalhador e a nenhuma obra.
    * Exception: Devolve uma exceção se um Capacete com o mesmo número "nCapacete" já existir.
    */
    Task Add(int nCapacete);
    
    /*
    Função que permite adicionar um capacete a uma obra.
    ie. adiciona uma obraId à obra
    Levanta uma exceção se o estado do capacete não for "Livre" e se o valor de "Obra" for diferente de null.
    */    
    Task AddCapaceteToObra(int nCapacete, string obraId);

    /*
    Função que permite remover a obra do Capacete.
    Levanta exceção se o capacete não estiver associado a nenhuma obra e se o capacete não estiver associado à obra indicada ou se não encontrar o capacete
    */
    Task RemoveCapaceteFromObra(int nCapacete, string obraId);

    Task UpdateCapaceteStatus(int nCapacete, string status);

    /*
    Função que permite mudar o Status do "nCapacete" para Livre.
    Levanta uma exceção se não poder mudar o estado do Capacete para Livre ou se não encontrar o capacete
    */
    Task UpdateCapaceteStatusToLivre(int nCapacete);

    /*
    Função que permite associar o trabalhador ao capacete.
    Levanta uma exceção se não encontrar o capacete ou se já tiver a associado a um trabalhador
    */
    Task AssociarTrabalhadorCapacete(int nCapacete, string idTrabalhador);

    /*
    Função que desassocia um trabalhador do capacete. 
    Levanta uma exceção se não encontrar o capacete, e se não estiver associado ao trabalhador indicado.
    */
    Task DesassociarTrabalhadorCapacete(int nCapacete, string idTrabalhador);
}
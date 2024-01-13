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
    */
    Task<bool> CheckIfCapaceteIsBeingUsed(int nCapacete);

    /**
    * Função que permite adicionar um novo Capacete com o número "nCapacete" ao sistema.
    * O estado inicial deste capacete será "Livre" e não estará associado a nenhum trabalhador.
    * Exception: Devolve uma exceção se um Capacete com o mesmo número "nCapacete" já existir.
    */
    Task Add(int nCapacete);
    

    
    Task AddCapaceteToObra(int nCapacete);

    Task UpdateCapaceteStatusToLivre(int nCapacete);

    Task UpdateCapaceteStatus(int nCapacete, string status);

    Task AssociarTrabalhadorCapacete(int nCapacete, string idTrabalhador);

    Task DesassociarTrabalhadorCapacete(int nCapacete, string idTrabalhador);
}
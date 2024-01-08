namespace iHat.Model.Capacetes;

public interface ICapacetesService{

    /**
    * Função que permite obter todos os capacetes do sistema.
    * Returns: uma lista dos Capacetes
    */
    Task<List<Capacete>> GetAll();

    /**
    * Função que permite obter o capacete com o número de Capacete "nCapacete" indicado
    * Returns: Devolve o Capacete ou nulo
    */
    Task<Capacete?> GetById(int nCapacete);

    /**
    * Função que permite adicionar um novo Capacete com o número "nCapacete" ao sistema.
    * O estado inicial deste capacete será "Livre" e não estará associado a nenhum trabalhador.
    * Exception: Devolve uma exceção se um Capacete com o mesmo número "nCapacete" já existir.
    */
    Task Add(int nCapacete);










    Task<List<Capacete>> GetAllHelmetsFromList(List<int> listNCapacetes);

    Task AddCapaceteToObra(int nCapacete);

    Task<bool> CheckIfCapaceteExists(int nCapacete);

    Task<bool> CheckIfHelmetIfBeingUsed(int nCapacete);

    Task UpdateCapaceteStatusToLivre(int nCapacete);

    Task UpdateCapaceteStatus(int nCapacete, string status);

    Task<List<Capacete>> GetFreeHelmets();

    Task AssociarTrabalhadorCapacete(int nCapacete, string idTrabalhador);

    Task DesassociarTrabalhadorCapacete(int nCapacete, string idTrabalhador);
}
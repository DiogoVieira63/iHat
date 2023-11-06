namespace iHat.Model.Capacetes;

public interface ICapacetesFacade{
    Task<List<Capacete>> GetAll();
    Task<Capacete> GetById(string id);
    Task<List<Capacete>> GetAllCapacetesdaObra(string idObra);
    Task Add(Capacete capacete);
    Task DeleteCapaceteToObra(string id, string idObra);
    Task AddCapaceteToObra(string idCapacete, string idObra);
}
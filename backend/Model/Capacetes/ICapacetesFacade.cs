namespace iHat.Model.Capacetes;

public interface ICapacetesFacade{
    Task<List<Capacete>> GetAll();
    Task<Capacete> GetById(int id);
    Task Add(Capacete capacete);
    Task Delete(int id);
    Task AddCapaceteToObra(int idCapacete, string idObra);
}
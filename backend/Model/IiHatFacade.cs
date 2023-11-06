using iHat.Model.Capacetes;
using iHat.Model.Obras;

public interface IiHatFacade{

    // Task NewConstruction(string name);

    Task<List<Obra>> GetObras(int idResponsavel);

    Task AddHelmet(Capacete capacete);

    Task<List<Capacete>> GetAll();

    Task<Capacete> GetCapacete(string id);

    Task<List<Capacete>> GetAllCapacetesdaObra(string idObra);

    Task DeleteCapaceteToObra(string id, string idObra);

    Task AddCapaceteToObra(string idCapacete, string idObra);
}
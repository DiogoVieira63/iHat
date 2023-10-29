using iHat.Model.Capacetes;

public interface IiHatFacade{

    // Task NewConstruction(string name);

    // Task<List<Obra>> GetObras(int idResponsavel);

    Task AddHelmet(Capacete capacete);

    Task<List<Capacete>> GetAll();

    Task<Capacete> GetCapacete(int id);

    Task Delete(int id);

    Task AddCapaceteToObra(int idCapacete, string idObra);
}
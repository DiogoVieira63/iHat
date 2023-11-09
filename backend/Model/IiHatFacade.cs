using iHat.Model.Capacetes;
using iHat.Model.Obras;

public interface IiHatFacade{

<<<<<<< HEAD
    // Task NewConstruction(string name);
=======
>>>>>>> aa5f5c9dc00d52f2d390e7b1fd0eae0bd7299053

    void NewConstruction(string name);
    Task<List<Obra>> GetObras(int idResponsavel);
<<<<<<< HEAD

    Task AddHelmet(Capacete capacete);

    Task<List<Capacete>> GetAll();

    Task<Capacete> GetCapacete(string id);

    Task<List<Capacete>> GetAllCapacetesdaObra(string idObra);

    Task DeleteCapaceteToObra(string id, string idObra);

    Task AddCapaceteToObra(string idCapacete, string idObra);
=======
    void AlteraEstadoObra(string id, string estado);
    Task NewConstruction(string name, string mapa, string status);

    Task<List<Obra>?> GetObras(int idResponsavel);

    Task<Obra> GetConstructionById(string id);
>>>>>>> aa5f5c9dc00d52f2d390e7b1fd0eae0bd7299053
}
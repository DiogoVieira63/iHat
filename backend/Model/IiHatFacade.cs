using iHat.Model.Capacetes;
using iHat.Model.Logs;
using iHat.Model.Obras;

public interface IiHatFacade{
    Task NewConstruction(string name, string mapa, string status);

    Task<List<Obra>?> GetObras(int idResponsavel);

    Task RemoveObraById(string obraId);

    Task<Obra> GetConstructionById(string id);

    Task AddHelmet(Capacete capacete);

    Task<List<Capacete>> GetAll();

    Task<Capacete> GetCapacete(string id);

    Task<List<Capacete>> GetAllCapacetesdaObra(string idObra);

    Task DeleteCapaceteToObra(string id, string idObra);

    Task AddCapaceteToObra(string idCapacete, string idObra);

    void AlteraEstadoObra(string id, string estado);

    void UpdateNomeObra(string idObra, string nome);

    Task <List<Log>> GetLogs(string idObra);

    Task AddLogs(Log logs);

}
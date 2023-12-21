using iHat.Model.Capacetes;
using iHat.Model.Logs;
using iHat.Model.Obras;
using iHat.Model.Zonas;

public interface IiHatFacade{
    Task NewConstruction(string name, IFormFile? mapa, int idResponsavel);

    Task<List<Obra>?> GetObras(int idResponsavel);    

    Task RemoveObraById(string obraId);

    Task<Obra> GetConstructionById(string id);

    Task<List<Capacete>> GetAllCapacetesdaObra(string idObra);

    Task DeleteCapaceteToObra(int nCapacete, string idObra);

    Task AddCapaceteToObra(int nCapacete, string idObra);

    void AlteraEstadoObra(string id, string estado);

    Task UpdateNomeObra(string idObra, string nome);

    Task<List<Log>> GetLogs(string idObra);

    Task AddLogs(Log logs);

    Task ChangeStatusCapacete(int nCapacete, string newStatus);





    Task<List<Capacete>> GetAllCapacetes();

    Task<Capacete?> GetCapacete(int nCapacete);

    Task AddCapacete(int nCapacete );

    Task<List<Capacete>> GetFreeHelmets();

    Task UpdateZonasRiscoObra(string idObra, string idMapa, List<ZonasRisco> zonas);
}
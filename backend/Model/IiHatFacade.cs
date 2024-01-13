using iHat.MensagensCapacete.Values;
using iHat.Model.Capacetes;
using iHat.Model.Logs;
using iHat.Model.Mapas;
using iHat.Model.MensagensCapacete;
using iHat.Model.Obras;
using iHat.Model.Zonas;

public interface IiHatFacade{

    // Obras
    Task<string?> NewConstruction(string name, IFormFile? mapa, int idResponsavel);

    Task<List<Obra>?> GetObras(int idResponsavel);    

    Task<Obra> GetConstructionById(string id);

    Task<List<Capacete>> GetAllCapacetesdaObra(string idObra);

    Task RemoveObraById(string obraId);

    Task DeleteCapaceteToObra(int nCapacete, string idObra);

    Task AddCapaceteToObra(int nCapacete, string idObra);

    Task AlteraEstadoObra(string id, string estado);

    Task UpdateNomeObra(string idObra, string nome);


    // Capacetes

    Task<List<Capacete>> GetAllCapacetes();

    Task<Capacete?> GetCapacete(int nCapacete);

    Task<List<Capacete>> GetFreeHelmets();

    Task ChangeStatusCapacete(int nCapacete, string newStatus);

    Task AddCapacete(int nCapacete );

    Task<List<MensagemCapacete>?> GetUltimosDadosDoCapacete(int nCapacete);

    Task<Dictionary<int, Location>> GetLastLocationCapacetesObra(string obraId);


    // Mapa

    Task UpdateZonasRiscoObra(string idObra, string idMapa, List<ZonasRisco> zonas);

    Task<List<Mapa>> GetMapasDaObra(List<string> listaMapasIds);

    Task AddMapa(string idObra, IFormFile mapaFile);

    Task UpdateMapaFloorNumber(string idObra, Dictionary<string, int> newFloors);


    // Logs

    Task<List<Log>> GetLogs(string idObra);

    Task<List<Log>> GetLogsByDate(string idObra, DateTime date);

    Task AddLogs(Log logs);

}
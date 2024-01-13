using iHat.MensagensCapacete.Values;
using iHat.Model.Capacetes;
using iHat.Model.Logs;
using iHat.Model.Mapas;
using iHat.Model.MensagensCapacete;
using iHat.Model.Obras;
using iHat.Model.Zonas;

public interface IiHatFacade{
    Task<string?> NewConstruction(string name, IFormFile? mapa, int idResponsavel);

    Task<List<Obra>?> GetObras(int idResponsavel);    

    Task RemoveObraById(string obraId);

    Task<Obra> GetConstructionById(string id);

    Task<List<Capacete>> GetAllCapacetesdaObra(string idObra);

    Task DeleteCapaceteToObra(int nCapacete, string idObra);

    Task AddCapaceteToObra(int nCapacete, string idObra);

    Task AlteraEstadoObra(string id, string estado);

    Task UpdateNomeObra(string idObra, string nome);

    

    Task<List<Log>> GetLogs(string idObra);

    Task<List<Log>> GetLogsByDate(string idObra, DateTime date);

    Task<List<Log>> GetDailyLogsCapacete(string idobra, int nCapacete);

    Task AddLogs(Log logs);

    

    Task ChangeStatusCapacete(int nCapacete, string newStatus);

    Task<List<Capacete>> GetAllCapacetes();

    Task<Capacete?> GetCapacete(int nCapacete);

    Task AddCapacete(int nCapacete );

    Task<List<Capacete>> GetFreeHelmets();

    Task UpdateZonasRiscoObra(string idObra, string idMapa, List<ZonasRisco> zonas);

    Task<List<Mapa>> GetMapasDaObra(List<string> listaMapasIds);

    Task AddMapa(string idObra, IFormFile mapaFile);

    Task<List<MensagemCapacete>?> GetUltimosDadosDoCapacete(int nCapacete);

    Task UpdateMapaFloorNumber(string idObra, Dictionary<string, int> newFloors);

    Task<Dictionary<int, Location>> GetLastLocationCapacetesObra(string obraId);
}
using iHat.Model.Zonas;

namespace iHat.Model.Obras;

public interface IObrasService{
    Task<List<Obra>> GetObrasOfResponsavel(int idResponsavel);

    Task RemoveObraByIdAsync(string obraId);

    void AlteraEstadoObra(string id, string estado);

    Task AddObra(string name, int idResponsavel, List<string> mapa);
    
    Task<Obra> GetConstructionById(string idObra);

    Task UpdateNomeObra(string idObra, string nome);

    // Task AddZonasPerigo(string idObra, List<Tuple<double,double>> lista);

    // Task RemoveZonasPerigo(string idObra);

    // Task RemoveAllZonasPerigo(string idObra);

    // Task UpdateZonasPerigo(string idObra, List<Tuple<double,double>> lista);

    Task<string?> GetIdObraWithCapaceteId(int nCapaceteToFind);

    Task<List<int>> GetAllCapacetesOfObra(string idObra);

    Task<bool> CheckIfObraExists(string idObra);

    Task AddCapaceteToObra(int idCapacete, string idObra);
    
    Task DeleteCapaceteToObra(int id, string idObra);

    Task UpdateZonasRiscoObra(string idObra, string idMapa, List<ZonasRisco> zonas);
}
    
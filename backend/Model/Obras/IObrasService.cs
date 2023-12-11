namespace iHat.Model.Obras;

public interface IObrasService{
    Task<List<Obra>> GetObrasOfResponsavel(int idResponsavel);

    Task RemoveObraByIdAsync(string obraId);


    void AlteraEstadoObra(string id, string estado);

    Task AddObra(string name, int idResponsavel, string mapa, string status);
    
    Task<Obra> GetConstructionById(string idObra);

    Task UpdateNomeObra(string idObra, string nome);

    Task AddZonasPerigo(string idObra, List<Tuple<double,double>> lista);

    Task RemoveZonasPerigo(string idObra);

    Task RemoveAllZonasPerigo(string idObra);

    Task UpdateZonasPerigo(string idObra, List<Tuple<double,double>> lista);

}
    
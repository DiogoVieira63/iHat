namespace iHat.Model.Obras;

public interface IObrasService{
    Task<List<Obra>> GetObrasOfResponsavel(int idResponsavel);

    void AlteraEstadoObra(string id, string estado);

    Task AddObra(string name, int idResponsavel, string mapa, string status);
    
    Task<Obra> GetConstructionById(string idObra);

    void UpdateNomeObra(string idObra, string nome);
}
    
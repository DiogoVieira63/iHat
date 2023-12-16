namespace iHat.Model.Obras;

public interface IObrasService{
    Task<List<Obra>> GetObrasOfResponsavel(int idResponsavel);

    Task RemoveObraByIdAsync(string obraId);


    Task AlteraEstadoObra(string id, string estado);

    Task AddObra(string name, int idResponsavel, string mapa, string status);
    
    Task<Obra> GetConstructionById(string idObra);

    Task UpdateNomeObra(string idObra, string nome);

}
    
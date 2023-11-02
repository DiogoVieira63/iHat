namespace iHat.Model.Obras;

public interface IObrasService{
    Task<List<Obra>> GetObrasOfResponsavel(int idResponsavel);
    Task AddObra(string name, int idResponsavel, string mapa, string status);
}
    
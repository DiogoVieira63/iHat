namespace iHat.Model.Obras;

public interface IObrasService{
    Task<List<Obra>> GetObrasOfResponsavel(int idResponsavel);
    void AddObra(string name);

    Task<Obra> GetObra(string idObra);
}
    
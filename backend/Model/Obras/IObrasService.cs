namespace iHat.Model.Obras;

public interface IObrasService{
    Task<List<Obra>> GetObrasOfResponsavel(int idResponsavel);
    void AddObra(string name);

    void AlteraEstadoObra(string id, string estado);

}
    
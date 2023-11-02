using iHat.Model.Obras;

public interface IiHatFacade{

    void NewConstruction(string name);
    Task<List<Obra>> GetObras(int idResponsavel);
    void AlteraEstadoObra(string id, string estado);
}
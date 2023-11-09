using iHat.Model.Obras;

public interface IiHatFacade{

    void NewConstruction(string name);

    Task<List<Obra>> GetObras(int idResponsavel);

    Task RemoveObraById(string obraId);
}
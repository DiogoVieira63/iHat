using iHat.Model.Obras;

public interface IiHatFacade{

    Task NewConstruction(string name);

    Task<List<Obra>> GetObras(int idResponsavel);
}
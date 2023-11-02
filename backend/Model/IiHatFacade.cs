using iHat.Model.Obras;

public interface IiHatFacade{

    Task NewConstruction(string name, string mapa, string status);

    Task<List<Obra>?> GetObras(int idResponsavel);

    Task<Obra> GetConstructionById(string id);
}
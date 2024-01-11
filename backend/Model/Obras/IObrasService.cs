using iHat.Model.Zonas;

namespace iHat.Model.Obras;

public interface IObrasService{
    /*
    Função que permite obter todas as obras de um responsável com id igual a idResponsavel
    */
    Task<List<Obra>> GetObrasOfResponsavel(int idResponsavel);

    /*
    Função que permite obter uma obra com o Id igua a idObra 
    */
    Task<Obra?> GetConstructionById(string idObra);
    /*
    Função que permite adicionar uma obra
    */
    Task<string?> AddObra(string name, int idResponsavel, List<string> mapa);
    /*
    Função que permite remover a obra com o id indicado
    */
    Task RemoveObraByIdAsync(string obraId);





    Task AlteraEstadoObra(string id, string estado);


    Task UpdateNomeObra(string idObra, string nome);

    // Task AddZonasPerigo(string idObra, List<Tuple<double,double>> lista);

    // Task RemoveZonasPerigo(string idObra);

    // Task RemoveAllZonasPerigo(string idObra);

    // Task UpdateZonasPerigo(string idObra, List<Tuple<double,double>> lista);

    Task<Obra?> GetObraWithCapaceteId(int nCapaceteToFind);

    Task<List<int>> GetAllCapacetesOfObra(string idObra);

    Task<bool> CheckIfObraExists(string idObra);

    Task AddCapaceteToObra(int idCapacete, string idObra);
    
    Task DeleteCapaceteToObra(int id, string idObra);

    Task UpdateZonasRiscoObra(string idObra, string idMapa, List<ZonasRisco> zonas);
    Task<List<string>> AddListaMapaToObra(string id, List<string> mapas);
}
    
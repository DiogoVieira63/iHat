using iHat.Model.Zonas;

namespace iHat.Model.Mapas;

public interface IMapaService{

    /*
    Função que permite obter a obra a partir do id.
    Retorna a obra ou null
    */
    Task<Mapa?> GetMapaById(string id);

    /*
    Função que obtem as Zonas de Risco de uma mapa "id".
    Retorna null se não encontrar o mapa ou uma lista de Zonas de Risco.
    */
    // Task<List<ZonasRisco>?> GetZonasdeRisco(string id);

    /*
    Função que permite adicionar um novo Mapa sem Zonas de Risco.
    Retonar o id do mapa criado e adicionado.
    */    
    Task<string?> Add(string name, string svg, int floor);
        
    /*
    Função que permite remover as zonas de perigo do mapa com o nome "name"
    Levanta uma exceção caso o mapa não existir
    */
    Task RemoveZonasPerigoOfMapa(string name);

    /*
    Função que permite remover todos os mapas da na lista "mapas"
    */
    Task RemoveMapas(List<string> mapas);

    /*
    Função que permite atualizar a lista de Zonas de Risco de um mapa "id".
    Levanta uma exceção se não encontrar o mapa "id"
    */
    Task UpdateZonasPerigoOfMapa(string id, List<ZonasRisco> zonas);

    /*
    Função que permite atualizar o número "Floor" de uma mapa "id" para "newFloorNumber".
    Levanta uma exceção se não encontrar o mapa "id"
    */
    Task UpdateFloorNumber(string id, int newFloorNumber);

}

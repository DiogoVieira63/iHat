using iHat.Model.Zonas;

namespace iHat.Model.Mapas;

public interface IMapaService{
    Task<string?> Add(string name, string svg, int floor);
        
    Task AddZoneRiscotoMapa(string name, List<ZonasRisco> lista);

    Task RemoveZonasPerigotoMapa(string name);

    Task RemoveAllZonasPerigotoMapa( string name);

    Task UpdateZonasPerigotoMapa(string Name, List<ZonasRisco> lista);

    Task<Mapa?> GetMapaById(string id);

    Task<List<ZonasRisco>?> GetZonasdeRisco(string id);

    Task RemoveMapas(List<string> mapas);
}

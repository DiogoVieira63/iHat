using iHat.Model.Zonas;

namespace iHat.Model.Mapas;

public interface IMapaService{
    Task<string?> Add(string name, string svg);
    
    Task AddZoneRiscotoMapa(string name, List<ZonasRisco> lista);

    Task RemoveZonasPerigotoMapa(string name);

    Task RemoveAllZonasPerigotoMapa( string name);

    Task UpdateZonasPerigotoMapa(string Name, List<ZonasRisco> lista);
}
namespace iHat.Model.Zonas;

public interface IZonasService{
    Task AddZonasPerigo(string idZona, List<Point> lista);

    Task RemoveZonasPerigo(string idZona);

    Task RemoveAllZonasPerigo(string idZona);

    Task UpdateZonasPerigo(string idZona, List<Point> lista);
}
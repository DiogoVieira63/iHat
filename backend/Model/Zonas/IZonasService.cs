namespace iHat.Model.Zonas;

public interface IZonasService{
    Task AddZonasPerigo(int idZona, List<Point> lista);

    Task RemoveZonasPerigo(int idZona);

    Task RemoveAllZonasPerigo(int idZona);

    Task UpdateZonasPerigo(int idZona, List<Point> lista);
}
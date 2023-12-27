namespace iHat.Model.Zonas;

public interface IZonasService{
    Task AddZonasPerigo(int idZona, List<Tuple<double,double>> lista);

    Task RemoveZonasPerigo(int idZona);

    Task RemoveAllZonasPerigo(int idZona);

    Task UpdateZonasPerigo(int idZona, List<Tuple<double,double>> lista);
}
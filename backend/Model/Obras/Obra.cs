using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace iHat.Model.Obras;

public class Obra
{
    [BsonId] // Primary key
    [BsonRepresentation(BsonType.ObjectId)] // permite passar uma variável do tipo ObjectId como string
    public string? Id { get; set; }
    public int IdResponsavel { get; set; }
    public string Name { get; set; }
    public List<Tuple<double, double>> Zonas { get; set; } //GeoJSON 
    public string Mapa { get; set; }
    public List<int> Capacetes { get; set; }
    public string Status { get; set; } // Finalizada; Pendente; Em Curso; Planeada; Cancelada

    public static readonly string Planeada = "Planeada";
    public static readonly string Pendente = "Pendente";
    public static readonly string EmCurso = "Em Curso";
    public static readonly string Cancelada = "Cancelada";
    public static readonly string Finalizada = "Finalizada";

    public Obra(string name, int idResponsavel, string mapa, string status)
    {
        this.IdResponsavel = idResponsavel;
        this.Name = name;
        this.Zonas = new List<Tuple<double, double>>();
        this.Mapa = mapa;
        this.Capacetes = new List<int>();
        this.Status = status;
    }
}
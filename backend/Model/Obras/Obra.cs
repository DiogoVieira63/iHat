using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using iHat.Model.Mapas;
namespace iHat.Model.Obras;

public class Obra
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public int IdResponsavel { get; set; }
    public string Name { get; set; }
    public List<string> Mapa { get; set; }
    public List<int> Capacetes { get; set; }
    public string Status { get; set; } // Finalizada; Pendente; Em Curso; Planeada; Cancelada

    public static readonly string Planeada = "Planeada";
    public static readonly string Pendente = "Pendente";
    public static readonly string EmCurso = "Em Curso";
    public static readonly string Cancelada = "Cancelada";
    public static readonly string Finalizada = "Finalizada";

    public Obra(string name, int idResponsavel)
    {
        IdResponsavel = idResponsavel;
        Name = name;
        Mapa = new List<string>();
        Capacetes = new List<int>();
        Status = Planeada;
    }
}
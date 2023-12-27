using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using iHat.Model.Zonas;

namespace iHat.Model.Mapas;

public class Mapa
{
    [BsonId] // Primary key
    [BsonRepresentation(BsonType.ObjectId)] // permite passar uma variável do tipo ObjectId como string
    public string? Id { get; set; }
    public string Name { get; set; }
    public string Svg { get; set; } // rever
    public List<ZonasRisco> Zonas { get; set; } // rever


    public Mapa(string name, string svg){
        Name = name;
        Svg = svg;
        Zonas = new List<ZonasRisco>();
    }
}
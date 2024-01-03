using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using iHat.Model.Zonas;

namespace iHat.Model.Mapas;

public class Mapa
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Name { get; set; }
    public string Svg { get; set; }
    public int Floor { get; set; }
    public List<ZonasRisco> Zonas { get; set; }


    public Mapa(string name, string svg, int floor){
        Name = name;
        Svg = svg;
        Floor = floor;
        Zonas = new List<ZonasRisco>();
    }
}
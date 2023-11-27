using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace iHat.Model.Obras;

public class Obra
{
    [BsonId] // Primary key
    [BsonRepresentation(BsonType.ObjectId)] // permite passar uma variável do tipo ObjectId como string
    public string? Id { get; set; } // string?

    // [BsonElement("Name")]: Nome na "tabela" do mongoDB
    public int IdResponsavel { get; set; }
    public string Name { get; set; }
    public List<string> Zonas { get; set; } //GeoJSON 
    public string Mapa { get; set; }
    public List<string> Capacetes { get; set; }
    public string Status { get; set; } // Finalizada; Pendente; Em Curso; Planeada; Cancelada

    public Obra(string name, int idResponsavel, string mapa, string status){
        this.IdResponsavel = idResponsavel;
        this.Name = name;
        this.Zonas = new List<string>();
        this.Mapa = mapa;
        this.Capacetes = new List<string>();
        this.Status = status;
    }
}
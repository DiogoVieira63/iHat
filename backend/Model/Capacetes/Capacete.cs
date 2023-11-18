using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace iHat.Model.Capacetes;

public class Capacete
{
    [BsonId] // Primary key
    [BsonRepresentation(BsonType.ObjectId)] // permite passar uma variável do tipo ObjectId como string
    public string? Id { get; set; }
    public string Status { get; set; }
    public string Info { get; set; }
    public string Obra { get; set; } //id da obra 
    public string Trabalhador { get; set; } // verificar isto se é string ou uma classe

    public Capacete(string status, string info, string obra, string trabalhador)
    {
        Status = status;
        Info = info;
        Obra = obra;
        Trabalhador = trabalhador;
    }
}
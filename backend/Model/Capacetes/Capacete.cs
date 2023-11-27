using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace iHat.Model.Capacetes;

public class Capacete
{
    [BsonId] // Primary key
    [BsonRepresentation(BsonType.ObjectId)] // permite passar uma variável do tipo ObjectId como string
    public string? Id { get; set; }

    public int NCapacete { get; set; }
    public string Status { get; set; }
    public string Info { get; set; }
    public string Trabalhador { get; set; } // verificar isto se é string ou uma classe

    public Capacete(int nCapacete, string status, string info, string trabalhador)
    {
        NCapacete = nCapacete;
        Status = status;
        Info = info;
        Trabalhador = trabalhador;
    }
}
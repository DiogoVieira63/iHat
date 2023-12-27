using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace iHat.Model.Capacetes;

public class Capacete
{
    [BsonId] 
    [BsonRepresentation(BsonType.ObjectId)] 
    public string? Id { get; set; }
    public int NCapacete { get; set; } 
    public string Status { get; set; }
    public string? Trabalhador { get; set; } 

    public static readonly string Livre = "Livre";
    public static readonly string AssociadoObra = "Associado à Obra";
    public static readonly string EmUso = "Em Uso";
    public static readonly string NaoOperacional = "Não Operacional";

    public Capacete(int nCapacete, string status, string? trabalhador)
    {
        NCapacete = nCapacete;
        Status = status;
        Trabalhador = trabalhador;
    }
}
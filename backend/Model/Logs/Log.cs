namespace iHat.Model.Logs;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Log
{
    [BsonId] // Primary key
    [BsonRepresentation(BsonType.ObjectId)] // permite passar uma variável do tipo ObjectId como string
    public string? Id {get; set;}
    public DateTime Timestap {get; set;}
    public string? IdObra {get; set;}
    public int? IdCapacete {get; set;}
    public string? IdTrabalhador {get; set;}
    public string? Mensagem {get; set;}

    
    public Log(DateTime timestap, string? idObra, int idCapacete, string idTrabalhador, string mensagem )
    {
        Timestap = timestap;
        IdObra = idObra;
        IdCapacete = idCapacete;
        IdTrabalhador = idTrabalhador;
        Mensagem = mensagem;
    }
}


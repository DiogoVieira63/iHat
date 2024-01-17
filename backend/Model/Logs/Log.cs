namespace iHat.Model.Logs;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Log
{
    [BsonId] // Primary key
    [BsonRepresentation(BsonType.ObjectId)] // permite passar uma variável do tipo ObjectId como string
    public string? Id {get; set;}
    public string Type {get; set;} // tipo de gravidade
    public DateTime Timestamp {get; set;}
    public string? IdObra {get; set;}
    public int? IdCapacete {get; set;}
    public string? IdTrabalhador {get; set;}
    public string? Mensagem {get; set;}
    public bool Vista {get; set;}

    /*public Log(DateTime Timestamp, string? idObra, int idCapacete, string idTrabalhador, string mensagem )
    {
        Timestamp = Timestamp;
        IdObra = idObra;
        IdCapacete = idCapacete;
        IdTrabalhador = idTrabalhador;
        Mensagem = mensagem;
    }*/

    public Log( string type, DateTime timestamp, string? idObra, int idCapacete, string idTrabalhador, string tipo){
        
        Type = type;
        Timestamp = timestamp;
        IdObra = idObra;
        IdCapacete = idCapacete;
        IdTrabalhador = idTrabalhador;
        Vista = false;

        switch(tipo){
            case "Fall":
                Mensagem = "Warning: Fall detected!";
                break;

            case "Temperature":
                Mensagem = "Warning: Unusual body temperature detected!";
                break;

            case "Heartrate":
                Mensagem = "Warning: Unusual heartrate detected!";
                break;

            case "Gases":
                Mensagem = "Warning: High concentration of harmful gases detected!";
                break;

            case "InsideZonaRisco":
                Mensagem = "Warning: Inside Risk Zone";
                break;

            default:
                break;
        }

    }
}


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace iHat.Model.Capacetes;

public class Capacete
{
    [BsonId] 
    [BsonRepresentation(BsonType.ObjectId)] 
    public string? Id { get; set; }
    public int Numero { get; set; } 
    public string Status { get; set; }
    public string? Trabalhador { get; set; } 
    public string? Obra { get; set; }
    public static readonly string Livre = "Livre";
    public static readonly string EmUso = "Em Uso";
    public static readonly string NaoOperacional = "Não Operacional";

    public Capacete(int numero)
    {
        Numero = numero;
        Status = Livre;
        Obra = null;
        Trabalhador = null;
    }

    public bool CanAddTrabalhador(){
        return Trabalhador == null && Status == Livre;
    }

    public bool CanBeAddedToObra(){
        return Status == Livre && Obra != null;
    }

    public bool CanRemoveTrabalhador(string trabalhador){
        return Trabalhador == trabalhador && Status == EmUso;
    }

    public bool CanReceiveMensagensCapacete(){
        return Status == EmUso;
    }

    public bool CanUpdateStatusToLivre(){
        return Status != Livre;
    }

    public bool CanUpdateStatusToEmUso(){
        return Status == Livre;
    }

    public bool CanUpdateStatusToNaoOperacional(){
        return Status != NaoOperacional;
    }
}
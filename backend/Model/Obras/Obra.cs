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
    public string Nome { get; set; }
    public List<string> Mapa { get; set; }
    public List<int> Capacetes { get; set; }
    public string Status { get; set; }
    public static readonly string Pendente = "Pendente";
    public static readonly string EmCurso = "Em Curso";
    public static readonly string Cancelada = "Cancelada";
    public static readonly string Finalizada = "Finalizada";

    public Obra(string name, int idResponsavel, List<string> mapas)
    {
        IdResponsavel = idResponsavel;
        Nome = name;
        Mapa = mapas;
        Capacetes = new List<int>();
        Status = Pendente;
    }

    public bool CanChangeName(){
        return Status == Pendente || Status == EmCurso;
    }

    public bool CanChangeMap(){
        return Status == Pendente || Status == EmCurso;
    }

    public bool CanChangeZonaRisco(){
        return Status == Pendente || Status == EmCurso;
    }

    public bool CanAddCapacete(){
        return Status == Pendente || Status == EmCurso;
    }

    public bool CanReceiveMensagensCapacete(){
        return Status == EmCurso;
    }

    public bool CanChangeStatus(){
        return Status == Pendente || Status == EmCurso;
    }
}
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using iHat.Model.Mapas;
namespace iHat.Model.Obras;

public class ObrasDTO
{
    public string? Id { get; set; }
    public int IdResponsavel { get; set; }
    public string Name { get; set; }
    public List<Mapa> Mapa { get; set; }
    public List<int> Capacetes { get; set; }
    public string Status { get; set; } // Finalizada; Pendente; Em Curso; Planeada; Cancelada


    public ObrasDTO(Obra obra, List<Mapa> listaMapas)
    {
        Id = obra.Id;
        IdResponsavel = obra.IdResponsavel;
        Name = obra.Nome;
        Mapa = listaMapas;
        Capacetes = obra.Capacetes;
        Status = obra.Status;
    }
}
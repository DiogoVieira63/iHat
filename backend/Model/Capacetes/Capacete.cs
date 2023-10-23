using iHat.Model.Obras;

namespace iHat.Model.Capacetes;

public class Capacete
{
    public int Id { get; set; }
    public string Status { get; set; }
    public string Info { get; set; }
    public Obra Obra { get; set; }
    public string Trabalhador { get; set; } // verificar isto se é string ou uma classe
}
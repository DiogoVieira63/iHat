namespace iHat.Model.Logs;

public class Log
{
    public string? Id {get; set;}
    public string Timestap {get; set;}
    public string? IdObra {get; set;}
    public string? IdCapacete {get; set;}
    public string? IdTrabalhador {get; set;}

    public Log(string timestap, string idObra, string idCapacete, string idTrabalhador)
    {
        Timestap = timestap;
        IdObra = idObra;
        IdCapacete = idCapacete;
        IdTrabalhador = idTrabalhador;
    }
}


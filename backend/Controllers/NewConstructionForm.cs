using System;
using System.ComponentModel.DataAnnotations;

namespace FormEncode.Models;

public class NewConstructionForm{

    public string Name {get; set;}
    public string Mapa {get; set;}
    public string Status {get; set;}    

    public NewConstructionForm(string name, string mapa, string status){
        Name = name;
        Mapa = mapa;
        Status = status;
    }
}
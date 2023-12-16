namespace FormEncode.Models;

public class NewConstructionForm{

    public string Name {get; set;}
    public IFormFile? Mapa {get; set;}

    public NewConstructionForm(string name, IFormFile? mapa){
        Name = name;
        Mapa = mapa;
    }

    public NewConstructionForm(){
        
    }

    /*public NewConstructionForm(string name){
        Name = name;
        Mapa = null;
    }*/
}
using System;
using System.ComponentModel.DataAnnotations;

namespace FormEncode.Models;

public class NewHelmetForm{

    public int Numero {get; set;}

    public NewHelmetForm(int numero){
        Numero = numero;
    }
}
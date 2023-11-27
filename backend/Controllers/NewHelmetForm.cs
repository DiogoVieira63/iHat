using System;
using System.ComponentModel.DataAnnotations;

namespace FormEncode.Models;

public class NewHelmetForm{

    public int NCapacete {get; set;}

    public NewHelmetForm(int nCapacete){
        NCapacete = nCapacete;
    }
}
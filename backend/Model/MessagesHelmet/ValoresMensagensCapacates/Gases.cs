namespace iHat.MensagensCapacete.Values;

public class Gases : IDefaultValueHelmetMessage{

    public double Metano {get; set;}

    private double _maxMetano {get; set;}

    public double MonoxidoCarbono {get; set;}

    private double _maxMonoxidoCarbono {get; set;}

    public Gases (
        double metano,
        double monoxidoCarbono
    ){
        Metano = metano;
        MonoxidoCarbono = monoxidoCarbono;
    }

    public bool isAbnormalValue(){
        return Metano > _maxMetano && MonoxidoCarbono > _maxMonoxidoCarbono;
    }

    public override string ToString()
    {
        return "Metano: "+ Metano+ ", Monoxido de Carbono: "+MonoxidoCarbono;
    }

}
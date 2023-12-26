using Newtonsoft.Json;

namespace iHat.MensagensCapacete.Values;

public class Location : IDefaultValueHelmetMessage{

    public double X {get; set;}
    public double Y {get; set;}
    public double Z {get; set;}

    [JsonConstructor]
    public Location(
        double x,
        double y,
        double z
    ){
        X = x;
        Y = y;
        Z = z;
    }

    public bool isAbnormalValue(){
        return false;
    }

    public override string ToString(){
        return "X:"+X+", Y:"+Y+", Z:"+Z;
    }


}
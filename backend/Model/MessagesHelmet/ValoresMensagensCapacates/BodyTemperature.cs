using Newtonsoft.Json;

namespace iHat.MensagensCapacete.Values;

public class BodyTemperature : IDefaultValueHelmetMessage{

    public double Value {get; set;}

    private double _minValue;
    private double _maxValue;

    [JsonConstructor]
    public BodyTemperature(double bodyTemperature){
        Value = bodyTemperature;
    }


    public bool isAbnormalValue(){
        return false;
    }


}
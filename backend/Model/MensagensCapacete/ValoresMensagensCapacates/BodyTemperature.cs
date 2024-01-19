using Newtonsoft.Json;

namespace iHat.MensagensCapacete.Values;

public class BodyTemperature : IDefaultValueHelmetMessage{

    public double Value {get; set;}

    private double _minValue=34.5;
    private double _maxValue=37.4;

    [JsonConstructor]
    public BodyTemperature(double bodyTemperature){
        Value = bodyTemperature;
    }


    public bool isAbnormalValue(){
        return Value < _minValue || Value > _maxValue;
        
    }


}
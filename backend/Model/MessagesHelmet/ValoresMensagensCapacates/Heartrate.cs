namespace iHat.MensagensCapacete.Values;

public class Heartrate : IDefaultValueHelmetMessage{

    public double Value {get; set;}
    public double _minValue=60;
    public double _maxValue=180;

    public Heartrate(double heartrate){
        Value = heartrate;
    }

    public bool isAbnormalValue(){
        return Value < _minValue || Value > _maxValue;
    }

}
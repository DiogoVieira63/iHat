namespace iHat.MensagensCapacete.Values;

public class Heartrate : IDefaultValueHelmetMessage{

    public double Value {get; set;}
    private double _minValue=60;
    private double _maxValue=180;

    public Heartrate(double heartrate){
        Value = heartrate;
    }

    public bool isAbnormalValue(){
        return Value < _minValue || Value > _maxValue;
    }

}
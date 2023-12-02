namespace iHat.MensagensCapacete.Values;

public class Heartrate : IDefaultValueHelmetMessage{

    public double Value {get; set;}
    public double _minValue;
    public double _maxValue;

    public Heartrate(double heartrate){
        Value = heartrate;
    }

    public bool isAbnormalValue(){
        return false;
    }

}
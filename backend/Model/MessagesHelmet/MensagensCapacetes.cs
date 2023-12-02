using System.Reflection.Metadata;
using iHat.MensagensCapacete.Values;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace iHat.MensagensCapacete;


public class MensagensCapacetes{

    public string NCapacete { get; set; }
    public string Type { get; set; }
    public bool Fall { get; set; }
    public BodyTemperature BodyTemperature {get; set;}
    public Heartrate Heartrate { get; set; }
    public string Proximity { get; set; }
    public string Position { get; set; }
    public Location Location { get; set; }
    public Gases Gases { get; set; }

    [JsonConstructor]
    public MensagensCapacetes ( 
        string helmetNB,
        string typeMessage,
        bool fall,
        double bodyTemperature,
        double heartrate,
        string proximity,
        string position,
        Location location,
        Gases gases
    ){
        NCapacete = helmetNB;
        Type = typeMessage;
        Fall = fall;
        BodyTemperature = new BodyTemperature(bodyTemperature);
        Heartrate = new Heartrate(heartrate);
        Proximity = proximity;
        Position = position;
        Location = location;
        Gases = gases;
    }

    public bool SearchForAnormalValues(){
        if (Fall) // Se detetou que houve uma queda notifica
            return Fall;

        if (BodyTemperature.isAbnormalValue())
            return true;

        return false;
    }
}
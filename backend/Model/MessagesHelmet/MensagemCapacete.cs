using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using iHat.MensagensCapacete.Values;
using Newtonsoft.Json;

namespace iHat.Model.MensagensCapacete;

public class MensagemCapacete {

    [BsonId] 
    [BsonRepresentation(BsonType.ObjectId)] 
    public string? Id { get; set; }

    public DateTime timestamp { get; set; }

    public int NCapacete { get; set; }
    public string Type { get; set; }
    public bool Fall { get; set; }
    public BodyTemperature BodyTemperature {get; set;}
    public Heartrate Heartrate { get; set; }
    public string Proximity { get; set; }
    public string Position { get; set; }
    public Location Location { get; set; }
    public Gases Gases { get; set; }

    [JsonConstructor]
    public MensagemCapacete ( 
        int helmetNB,
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

    
    public Tuple<bool, string> SearchForAnormalValues(){
        if (Fall) // Se detetou que houve uma queda notifica
            return new Tuple<bool, string>(true, "Fall");
        if (BodyTemperature.isAbnormalValue())
            return new Tuple<bool, string>(true, "Temperature");
        if (Heartrate.isAbnormalValue())
            return new Tuple<bool, string>(true, "Heartrate");
        if (Gases.isAbnormalValue())
            return new Tuple<bool, string>(true, "Gases");
        return new Tuple<bool, string>(false, "");
        // localização com as zonas de perigo
    }
}
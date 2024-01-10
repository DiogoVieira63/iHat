using System;
using System.Collections;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class PublisherService{

    static JObject GetMessageContent(){
        var result = new JObject
        {
            { "HelmetNB", "1" },
            { "TypeMessage", "ValueUpdate" },
            { "Fall", "False" },
            { "BodyTemperature", 38 },
            { "Heartrate", 100 },
            { "Proximity", "10" },
            { "Position", "?" }
        };



        JObject loc = new JObject
        {
            { "X", 0 },
            { "Y", 0 },
            { "Z", 0 }
        };
        result.Add("Location", loc);

        JObject gases = new JObject
        {
            { "Metano", 0 },
            { "MonoxidoCarbono", 0 }
        };

        result.Add("Gases", gases);
        
        return result;
    } 

    
    static JObject GetPairingContent(){
        var result = new JObject
        {
            { "type", "Pairing" },
            { "nCapacete", "2" },
            { "idTrabalhador", "a02" },
            { "obra", "6590435fe8796e8765a895f0" }
        };        
        return result;
    }


    static async Task Main(string[] args)
    {
        // Create an MQTT client instance
        var factory = new MqttFactory();
        var mqttClient = factory.CreateMqttClient();

        // Create TCP based options using the builder pattern
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost",1883) // Replace with your broker's address
            .WithClientId("YourClientId") // Provide a unique client ID
            .WithCredentials("admin", "password")
            .Build();

        // Connect to the broker
        await mqttClient.ConnectAsync(options);

        var messagePayload = GetMessageContent();
        string json = JsonConvert.SerializeObject(messagePayload);
        byte[] serializedResult = Encoding.UTF8.GetBytes(json);
        var message = new MqttApplicationMessageBuilder()
            .WithTopic("my/topic")
            .WithPayload(serializedResult)
            .WithRetainFlag()
            .Build();
        await mqttClient.PublishAsync(message);

        messagePayload = GetPairingContent();
        json = JsonConvert.SerializeObject(messagePayload);
        serializedResult = Encoding.UTF8.GetBytes(json);
        message = new MqttApplicationMessageBuilder()
            .WithTopic("ihat/obras")
            .WithPayload(serializedResult)
            .WithRetainFlag()
            .Build();
        await mqttClient.PublishAsync(message);



        // Disconnect from the broker
        await mqttClient.DisconnectAsync();
    }



}


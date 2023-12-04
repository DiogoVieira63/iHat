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

    static JObject getMessageContent(){
        var result = new JObject();
        result.Add("HelmetNB", "1");
        result.Add("TypeMessage","ValueUpdate");
        result.Add("Fall", "False");
        result.Add("BodyTemperature", 36.1);
        result.Add("Heartrate", 100);
        result.Add("Proximity", "10");
        result.Add("Position", "?");



        JObject loc = new JObject();
        loc.Add("X", 0);
        loc.Add("Y", 0);
        loc.Add("Z", 0);
        result.Add("Location", loc);

        JObject gases = new JObject();
        gases.Add("Metano", 0);
        gases.Add("MonoxidoCarbono", 0);

        result.Add("Gases", gases);
        
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

        var messagePayload = getMessageContent();
        
        string json = JsonConvert.SerializeObject(messagePayload);
        byte[] serializedResult = System.Text.Encoding.UTF8.GetBytes(json);
            

        // Create and publish a message
        var message = new MqttApplicationMessageBuilder()
            .WithTopic("my/topic") // Replace with your desired topic
            .WithPayload(serializedResult)
            .WithRetainFlag()
            .Build();

        await mqttClient.PublishAsync(message);

        // Disconnect from the broker
        await mqttClient.DisconnectAsync();
    }



}


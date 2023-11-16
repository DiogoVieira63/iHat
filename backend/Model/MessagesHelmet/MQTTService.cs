using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;

public class MQTTService {

    private IManagedMqttClient _mqttClient;
    private ManagedMqttClientOptions _managedOptions;    
    private string _clientId = "iHatBackendServer"; // Replace with your Client Id
    private string _mqttBrokerAddress = "mqttBrokerAddress"; // Replace with your MQTT broker address
                                                             // MQTT broker is the backend system which coordinates messages between the different clients.                  

    public MQTTService()
    {
        var options = new MqttClientOptionsBuilder()
            .WithClientId(_clientId)
            .WithTcpServer(_mqttBrokerAddress) // WithTcpServer("localhost", 1883)
            .Build();

        _managedOptions = new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
            .WithClientOptions(options)
            .Build();

        _mqttClient = new MqttFactory().CreateManagedMqttClient();
        _mqttClient.ApplicationMessageReceivedAsync += HandleReceivedMessage;    
    
        /*_mqttClient.UseConnectedHandler(async e => {
            Console.WriteLine("Connected to MQTT broker.");
            var topicFilter = new MqttTopicFilterBuilder().WithTopic("test/topic").Build();
            await client.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopicFilter(topicFilter).Build());
        });
        _mqttClient.UseDisconnectedHandler(async e => {
            Console.WriteLine("Disconnected from MQTT broker.");
            await Task.Delay(TimeSpan.FromSeconds(5));
            try {
                await client.ConnectAsync(options, CancellationToken.None);
            } catch {
                Console.WriteLine("Reconnecting to MQTT broker failed.");
            }
        });*/
    }

    public async Task StartAsync(){
        try {
            await _mqttClient.StartAsync(_managedOptions);
        } catch {
            Console.WriteLine("Connecting to MQTT broker failed.");
        }
        
        string topic = "your/topic";
        var topicFilter = new MqttTopicFilterBuilder().WithTopic(topic).Build();

        var topicFilterCollection = new List<MQTTnet.Packets.MqttTopicFilter>
        {
            topicFilter
        };

        await _mqttClient.SubscribeAsync(topicFilterCollection);
    }

    public async Task StopAsync()
    {
        if (_mqttClient != null && _mqttClient.IsStarted)
            await _mqttClient.StopAsync();
    }

    private Task HandleReceivedMessage(MqttApplicationMessageReceivedEventArgs eventArgs)
    {
        // Handle incoming MQTT message
        var payload = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.PayloadSegment);
        Console.WriteLine($"Received message on topic '{eventArgs.ApplicationMessage.Topic}': {payload}");

        // Perform additional processing based on the received message

        return Task.CompletedTask;
    }



    /*
    static async Task Main(string[] args) {
        var factory = new MqttFactory();
        var client = factory.CreateMqttClient();
        var options = new MqttClientOptionsBuilder().WithTcpServer("localhost", 1883).WithClientId("mqtt_consumer").Build();


        client.UseConnectedHandler(async e => {
            Console.WriteLine("Connected to MQTT broker.");
            var topicFilter = new MqttTopicFilterBuilder().WithTopic("test/topic").Build();
            await client.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopicFilter(topicFilter).Build());
        });


        client.UseDisconnectedHandler(async e => {
            Console.WriteLine("Disconnected from MQTT broker.");
            await Task.Delay(TimeSpan.FromSeconds(5));
            try {
                await client.ConnectAsync(options, CancellationToken.None);
            } catch {
                Console.WriteLine("Reconnecting to MQTT broker failed.");
            }
        });


        client.UseApplicationMessageReceivedHandler(e => {
            Console.WriteLine($ "Received message on topic '{e.ApplicationMessage.Topic}': {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
        });


        try {
            await client.ConnectAsync(options, CancellationToken.None);
        } catch {
            Console.WriteLine("Connecting to MQTT broker failed.");
        }


        Console.ReadLine();
    }*/
}
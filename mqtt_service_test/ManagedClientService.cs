using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;

public class ManagedClientService{

    public async Task Main()
    {
        /*
         * This sample creates a simple managed MQTT client and connects to a public broker.
         *
         * The managed client extends the existing _MqttClient_. It adds the following features.
         * - Reconnecting when connection is lost.
         * - Storing pending messages in an internal queue so that an enqueue is possible while the client remains not connected.
         */
        
        var mqttFactory = new MqttFactory();

        using (var managedMqttClient = mqttFactory.CreateManagedMqttClient())
        {
            var options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost",1883) // Replace with your broker's address
            .WithClientId("YourClientId") // Provide a unique client ID
            .WithCredentials("admin", "password")
            .Build();

            var managedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(options)
                .Build();

            await managedMqttClient.StartAsync(managedMqttClientOptions);

            // The application message is not sent. It is stored in an internal queue and
            // will be sent when the client is connected.
            await managedMqttClient.EnqueueAsync("my/topic", "Payload");

            Console.WriteLine("The managed MQTT client is connected.");
            
            // Wait until the queue is fully processed.
            SpinWait.SpinUntil(() => managedMqttClient.PendingApplicationMessagesCount == 0, 10000);
            
            Console.WriteLine($"Pending messages = {managedMqttClient.PendingApplicationMessagesCount}");
        }
    }
}
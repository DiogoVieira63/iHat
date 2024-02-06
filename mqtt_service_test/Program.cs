using System.Text;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;

public class Program{
    
    private string url = "mqtt-broker";
    private int port = 1883;

    private string username = "admin";
    private string password = "password";

    private string topic = "my/topic";

    private static Task HandleReceivedMessage(MqttApplicationMessageReceivedEventArgs eventArgs)
    {
        var payload = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.PayloadSegment);
        Console.WriteLine("Received application message. {0}. {1}", payload, eventArgs.ApplicationMessage.Topic);
        return Task.CompletedTask;
    }

    private static async Task ConnectingAction(IMqttClient mqttClient, CancellationTokenSource timeoutToken){
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("mqtt-broker", 1883)
            .WithCredentials("admin", "password")
            .WithProtocolVersion(MqttProtocolVersion.V500)
            .Build();

        var response = await mqttClient.ConnectAsync(mqttClientOptions, timeoutToken.Token);
        Console.WriteLine("The MQTT client is connected. {0}", response.ToString());
        await Task.Delay(1000);
        // This will throw an exception if the server does not reply.
        await mqttClient.PingAsync(CancellationToken.None);
        Console.WriteLine("The MQTT server replied to the ping request.");
        await Task.Delay(1000);
    }

    private static async Task SubscribeTopic(IMqttClient mqttClient){
        var mqttFactory = new MqttFactory();
        
        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(
                f => {
                    f.WithTopic("my/topic");
                })
            .Build();

        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        Console.WriteLine("MQTT client subscribed to topic.");
    }

    private static async Task DisconnectingAction(IMqttClient mqttClient){
        // This will send the DISCONNECT packet. Calling _Dispose_ without DisconnectAsync the 
        // connection is closed in a "not clean" way. See MQTT specification for more details.
        await mqttClient.DisconnectAsync(
            new MqttClientDisconnectOptionsBuilder()
                .WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection)
                .Build()
        );
    }


    public static async Task Main2(string[] args)
    {
        Console.WriteLine("Starting...");
        await Task.Delay(1000);

        var mqttFactory = new MqttFactory();
        var mqttClient = mqttFactory.CreateMqttClient();
        var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(1));

        mqttClient.ApplicationMessageReceivedAsync += HandleReceivedMessage;

        mqttClient.DisconnectedAsync += async e =>
        {
            Console.WriteLine("Client Disconnected");
            await Task.Delay(1000);
            
            if (e.ClientWasConnected)
            {
                // Use the current options as the new options.
                await mqttClient.ConnectAsync(mqttClient.Options, timeoutToken.Token);
            }
        };
        
        try 
        {
            {
                await ConnectingAction(mqttClient, timeoutToken);

                await SubscribeTopic(mqttClient);
                
                Console.WriteLine("Press enter to exit.");
                Console.ReadLine();

                await DisconnectingAction(mqttClient);   
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Timeout while connecting.");
            await Task.Delay(1000);
        }


        Console.WriteLine("Ending...");
        await Task.Delay(1000);
    }
}
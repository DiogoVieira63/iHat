using System.Text;
using iHat.MensagensCapacete;
using iHat.Model.Capacetes;
using iHat.Model.Logs;
using iHat.Model.Obras;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace iHat.MQTTService;

public class MQTTService {

    private readonly string _topic = "my/topic";
    private readonly string _clientId = "iHatBackendServer";
    private readonly string _url = "localhost";
    private readonly int _port = 1883;
    private readonly string _username = "admin";
    private readonly string _password = "password";

    private ILogger<MQTTService> _logger;
    private IMqttClient _mqttClient;
    private MqttFactory _mqttFactory; 
    private CancellationTokenSource _timeoutToken;

    private ICapacetesService _capacetesService;
    private IObrasService _obrasService;
    private ILogsService _logsService;

    public MQTTService(ILogger<MQTTService> logger, ICapacetesService capacetesService, IObrasService obrasService, ILogsService logsService){

        _logger = logger;
        _obrasService = obrasService;
        _capacetesService = capacetesService;

        _mqttFactory = new MqttFactory();
        _mqttClient = _mqttFactory.CreateMqttClient();
        _timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(1));

        _mqttClient.ApplicationMessageReceivedAsync += HandleReceivedMessage;
        _mqttClient.DisconnectedAsync += async e =>
        {
            _logger.LogInformation("Disconnected from MQTT Broker...");
            
            if (e.ClientWasConnected)
            {
                // Use the current options as the new options.
                await _mqttClient.ConnectAsync(_mqttClient.Options, _timeoutToken.Token);
            }
        };

        
    }


    public async Task StartAsync(){
        try{
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(_url, _port)
                .WithClientId(_clientId)
                .WithCredentials(_username, _password)
                .WithProtocolVersion(MqttProtocolVersion.V500)
                .Build();

            var response = await _mqttClient.ConnectAsync(mqttClientOptions, _timeoutToken.Token);
            _logger.LogInformation("The MQTT client is connected. {0}", response.ToString());

            // This will throw an exception if the server does not reply.
            await _mqttClient.PingAsync(CancellationToken.None);
            _logger.LogInformation("The MQTT server replied to the ping request.");


            var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f => {
                        f.WithTopic(_topic);
                    })
                .Build();

            await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
            _logger.LogInformation("MQTT client subscribed to topic.");

        }
        catch(Exception e){
            _logger.LogInformation(e.Message);
        }
    }

    private async Task HandleReceivedMessage(MqttApplicationMessageReceivedEventArgs eventArgs)
    {
        var payload = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.PayloadSegment);
        _logger.LogInformation("Received application message. {0}. {1}", payload, eventArgs.ApplicationMessage.Topic);
    
        try {
            var messageJson = JsonConvert.DeserializeObject<MensagensCapacetes>(payload);

            if(messageJson is null){
                _logger.LogInformation("Message received could not be parsed.");
            }

            _logger.LogInformation(messageJson.NCapacete.ToString());
            _logger.LogInformation(messageJson.Location.ToString());
            _logger.LogInformation(messageJson.Gases.ToString());

            Tuple<bool, string> messageRe = messageJson.SearchForAnormalValues();

            if (messageRe.Item1 == true){
                var message = "";
                switch(messageRe.Item2){
                    case "Fall":
                        message = "Warning: Fall detected!";
                        break;

                    case "Temperature":
                        message = "Warning: Unusual body temperature detected!";
                        break;

                    case "Heartrate":
                        message = "Warning: Unusual heartrate detected!";
                        break;

                    case "Gases":
                        message = "Warning: High concentration of harmful gases detected!";
                        break;

                    default:
                        break;
                }

                // get helmet by NCapacete 
                var capacete = await  _capacetesService.GetById(messageJson.NCapacete);
                        
                // get idObra
                var obra = await _obrasService.GetIdObraWithCapaceteId(capacete.NCapacete);

                // (DateTime timestap, string idObra, string idCapacete, string idTrabalhador, string mensagem )
                var log = new Log(DateTime.Now, obra, messageJson.NCapacete, capacete.Trabalhador, message);
                
                // Save Log in DB
                await _logsService.Add(log);
                // Notify Helmet
                        
                // Notify FrontEnd

            }
               
        }catch(Exception e){
            Console.WriteLine(e.Message);
        }
    }


    public async Task StopAsync(){
        await _mqttClient.DisconnectAsync(
            new MqttClientDisconnectOptionsBuilder()
                .WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection)
                .Build()
        );
    }  

}
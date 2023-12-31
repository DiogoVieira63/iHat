using System.Text;
using iHat.Model.MensagensCapacete;
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
    private MensagemCapaceteService _mensagemCapaceteService;

    public MQTTService(ILogger<MQTTService> logger, ICapacetesService capacetesService, IObrasService obrasService, ILogsService logsService, MensagemCapaceteService mensagemCapaceteService){

        _logger = logger;
        _obrasService = obrasService;
        _capacetesService = capacetesService;
        _logsService = logsService;
        _mensagemCapaceteService = mensagemCapaceteService;

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
        _logger.LogDebug("Received application message. {0}. Topic: {1}", payload, eventArgs.ApplicationMessage.Topic);
    
        try {
            var messageJson = JsonConvert.DeserializeObject<MensagemCapacete>(payload);

            if(messageJson is null){
                _logger.LogDebug("Message received could not be parsed.");
                return;
            }

            messageJson.timestamp = DateTime.Now;

            // Verifica se há um capacete ao qual associar a mensagem
            var capacete = await _capacetesService.GetById(messageJson.NCapacete);
            if(capacete == null){
                _logger.LogWarning("Mensagem MQTT recebida não tem um Capacete associado.");
                return;
            }

            if(capacete.Trabalhador == null){
                _logger.LogWarning("Mensagem recebida de um capacete que não está associado a nenhum trabalhador");
                return;
            }

            if(capacete.Status != Capacete.EmUso){
                _logger.LogWarning("Mensagem recebida de um capacete que não está a ser utilizado");
                return;
            }

            await _mensagemCapaceteService.Add(messageJson);

            Tuple<bool, string> messageRe = messageJson.SearchForAnormalValues();
            if (messageRe.Item1 == true){
                _logger.LogInformation("Abnormal Value Detected.");              
                var obra = await _obrasService.GetIdObraWithCapaceteId(capacete.NCapacete);
                var type = string.Empty;
                switch(messageRe.Item2){
                    case "Fall":
                        type = "Grave";
                        break;
                    case "Temperature":
                        type = "Alerta";
                        break;
                    case "Heartrate":
                        type = "Alerta";
                        break;
                    case "Gases":
                        type = "Alerta";
                        break;
                    default:
                        break;
                }

                var log = new Log(type, DateTime.Now, obra, messageJson.NCapacete, capacete.Trabalhador, messageRe.Item2);
                await _logsService.Add(log);
                
                // Notify Helmet
                await NotifyCapacete(messageJson.NCapacete);
                        
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


    public async Task NotifyCapacete(int nCapacete){
        var messagePayload = new JObject
        {
            { "Notify", true }
        };

        string json = JsonConvert.SerializeObject(messagePayload);
        byte[] serializedResult = Encoding.UTF8.GetBytes(json);
    
        // Create and publish a message
        var message = new MqttApplicationMessageBuilder()
            .WithTopic("my/topic/"+nCapacete) // Replace with your desired topic
            .WithPayload(serializedResult)
            .WithRetainFlag()
            .Build();

        await _mqttClient.PublishAsync(message);
    
    }

}
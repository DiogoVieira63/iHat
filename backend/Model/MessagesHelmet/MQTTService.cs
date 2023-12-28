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
using iHat.MensagensCapacete.Values;
using iHat.Model.Mapas;


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
    private IMapaService _mapsService;

    public MQTTService(ILogger<MQTTService> logger, ICapacetesService capacetesService, IObrasService obrasService, ILogsService logsService, MensagemCapaceteService mensagemCapaceteService, IMapaService mapsService){

        _logger = logger;
        _obrasService = obrasService;
        _capacetesService = capacetesService;
        _logsService = logsService;
        _mensagemCapaceteService = mensagemCapaceteService;
        _mapsService = mapsService;

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

            var obra = await _obrasService.GetObraWithCapaceteId(capacete.NCapacete);
            if(obra == null){
                return;
            }

            Tuple<bool, string> messageRe = messageJson.SearchForAnormalValues();
            if (messageRe.Item1 == true){
                _logger.LogInformation("Abnormal Value Detected.");              
                var log = new Log(DateTime.Now, obra.Id, messageJson.NCapacete, capacete.Trabalhador, messageRe.Item2);
                await _logsService.Add(log);
                
                // Notify Helmet
                await NotifyCapacete(messageJson.NCapacete);
                        
                // Notify FrontEnd
            }

            var found = await CheckSeCapaceteEstaZonaRisco(obra, messageJson.Location);           
            if(found){
                await NotifyCapacete(messageJson.NCapacete);
            }



        }catch(Exception e){
            Console.WriteLine(e.Message);
        }
    }

    public async Task<bool> CheckSeCapaceteEstaZonaRisco(Obra obra, Location location){
        var naZona = false;

        var mapas = obra.Mapa;

        foreach (var mapaId in mapas){
            var mapa = await _mapsService.GetMapaById(mapaId);
            if(mapa != null && mapa.Floor == location.Z){
                var zonasRisco = mapa.Zonas;
                foreach(var zona in zonasRisco){
                    naZona &= zona.InsideZonaRisco(location.X, location.Y);
                }
            }        
        }
        return naZona;
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
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
using SignalR.Hubs;

namespace iHat.MQTTService;

public class MQTTService {

    private readonly string _BasicTopic = "my/topic";
    private readonly string _PairingTopic = "ihat/obras";
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
    private ManageNotificationClients _manageNotificationClients;
    

    public MQTTService(ILogger<MQTTService> logger, ICapacetesService capacetesService, IObrasService obrasService,
                       ILogsService logsService, MensagemCapaceteService mensagemCapaceteService, IMapaService mapsService,
                       ManageNotificationClients manageNotificationClients){

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
        _manageNotificationClients = manageNotificationClients;
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
                        f.WithTopic(_BasicTopic);
                        f.WithTopic(_PairingTopic);
                    })
                .Build();

            await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
            _logger.LogInformation("MQTT client subscribed to topic.");

        }
        catch(Exception e){
            _logger.LogInformation(e.Message);
        }
    }

    public async Task StopAsync(){
        await _mqttClient.DisconnectAsync(
            new MqttClientDisconnectOptionsBuilder()
                .WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection)
                .Build()
        );
    }  

    private async Task HandleReceivedMessage(MqttApplicationMessageReceivedEventArgs eventArgs)
    {
        var payload = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.PayloadSegment);
        _logger.LogWarning("Received application message. {0}. Topic: {1}", payload, eventArgs.ApplicationMessage.Topic);
    

        if(payload != null && eventArgs.ApplicationMessage.Topic.Equals(_BasicTopic)){
            await HandleMessageFromCapacetes(payload);
        }
        
        if(payload != null && eventArgs.ApplicationMessage.Topic.Equals(_PairingTopic)){
            await HandlePairingMessage(payload);
        }
    }

    public async Task HandleMessageFromCapacetes(string payload){
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
            await _manageNotificationClients.NotifyClientsCapaceteWithLastMessage(messageJson.NCapacete, messageJson);


            var obra = await _obrasService.GetObraWithCapaceteId(capacete.NCapacete);
            if(obra == null){
                return;
            }

            await NotifyClientsWithCapaceteLocation(capacete, messageJson, obra);
            

            Tuple<bool, string> messageRe = messageJson.SearchForAnormalValues();
            if (messageRe.Item1 == true){
                _logger.LogInformation("Abnormal Value Detected.");              
                // var obra = await _obrasService.GetIdObraWithCapaceteId(capacete.NCapacete);
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

                var log = new Log(type, DateTime.Now, obra.Id, messageJson.NCapacete, capacete.Trabalhador, messageRe.Item2);
                // var log = new Log(DateTime.Now, obra.Id, messageJson.NCapacete, capacete.Trabalhador, messageRe.Item2);
                await _logsService.Add(log);

                // Notify Frontend
                if(log.IdObra != null){
                    var listaLogs = await _logsService.GetLogsOfObra(log.IdObra);
                    await _manageNotificationClients.NotifyClientsObraWithAllLogs(log.IdObra, listaLogs);
                }
                
                // Notify Helmet
                await NotifyCapacete(messageJson.NCapacete);
            }

            var found = await CheckSeCapaceteEstaZonaRisco(obra, messageJson.Location);           
            if(found){
                await NotifyCapacete(messageJson.NCapacete);
            }
        }catch(Exception e){
            Console.WriteLine(e.Message);
        }
    }

    public async Task HandlePairingMessage(string payload){
        JObject jsonObject = JObject.Parse(payload);
        JToken? nCapacete = jsonObject["nCapacete"];
        JToken? idTrabalhador = jsonObject["idTrabalhador"];
        JToken? obra = jsonObject["obra"];
    
        if(nCapacete != null && idTrabalhador != null){
            int nCap = (int) nCapacete;
            string idTrab = (string) idTrabalhador;
            try{
                await _capacetesService.AssociarTrabalhadorCapacete(nCap, idTrab);
            }
            catch(Exception e){
                _logger.LogWarning(e.Message);
            }
            
        }
    }

    public async Task NotifyClientsWithCapaceteLocation(Capacete capacete, MensagemCapacete messageJson, Obra obra){
        // Primeira op: Notificar apenas com a nova localização recebida...
        var dict = new Dictionary<int, Location>
        {
            { capacete.NCapacete, messageJson.Location }
        };
        await _manageNotificationClients.NotifyClientsObraWithSingleLocation(obra.Id!, dict);        

        // Segunda op: Notificar com todas as últimas localizações...
        var listaCapacetes = obra.Capacetes;
        var allCapacetesLocation = new Dictionary<int, Location>();
        foreach(var id in listaCapacetes){
            var loc = await _mensagemCapaceteService.GetLastLocation(id);
            if (loc != null)
                allCapacetesLocation.Add(id, loc);
        }
        await _manageNotificationClients.NotifyClientsObraWithMultipleLocations(obra.Id!, allCapacetesLocation);
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
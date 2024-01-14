namespace iHat.Model.MQTTService;

public class MQTTBackgroundService: BackgroundService
{
    private readonly MQTTService _mqttService;
    private ILogger<MQTTBackgroundService> _logger;

    public MQTTBackgroundService(MQTTService mqttService, ILogger<MQTTBackgroundService> logger){
        this._mqttService = mqttService;
        this._logger = logger;
    }

    protected override async Task ExecuteAsync (CancellationToken stoppingToken){
        // Start the MQTT service in the background
        _logger.LogInformation("Starting the MQTT service in the background...");
        await _mqttService.StartAsync();
    }

    public override async Task StopAsync (CancellationToken stoppingToken){
        // Stop the MQTTService in the background
        _logger.LogInformation("Stopping the MQTT service in the background...");
        await _mqttService.StopAsync();
    }
}
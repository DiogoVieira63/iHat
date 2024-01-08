using Xunit;
using Moq;
using Microsoft.Extensions.Options;

namespace iHat.Model.Logs;

public class LogsServiceTests{

    private static ILogsService? Setup(){
        var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                 .AddEnvironmentVariables() 
                 .Build();

        var databaseSettings =  (DatabaseSettings?) config.GetValue(typeof(DatabaseSettings), "Database" );
        if(databaseSettings == null){
            return null;
        }
        IOptions<DatabaseSettings> options1 = Options.Create<DatabaseSettings>(databaseSettings);
        
        var logger = Mock.Of<ILogger<LogsService>>();

        var logsService = new LogsService(options1, logger);

        return logsService;
    }

    [Fact]
    public async void Test_AddLog(){
        var type = "Alerta";
        var timestamp = DateTime.Now;
        var idObra = "1";
        var idCapacete = 1;
        var idTrabalhador = "1";
        var tipo = "Fall";

        // Arrange
        var logsService = Setup();
        if(logsService == null)
            return;

        var log = new Log(type, timestamp, idObra, idCapacete, idTrabalhador, tipo);

        // Act
        await logsService.Add(log);

        var logs = await logsService.GetLogsOfObra(idObra);
        Assert.True(logs.Count > 0);
    }
}
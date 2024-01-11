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
        var timestamp = DateTime.Today;
        var idObra = "1";
        var idCapacete = 1;
        var idTrabalhador = "1";
        var tipo = "Fall";

        var type1 = "Grave";
        var timestamp1 = DateTime.Parse("03/01/2024");
        var idCapacete1 = 2;
        var idTrabalhador1 = "2";
        var tipo1 = "Fall";

        // Arrange
        var logsService = Setup();
        if(logsService == null)
            return;

        var log1 = new Log(type, timestamp, idObra, idCapacete, idTrabalhador, tipo);
        var log2 = new Log(type1, timestamp1, idObra, idCapacete1, idTrabalhador1, tipo1);
        // Act
        await logsService.Add(log1);
        await logsService.Add(log2);

        var logs = await logsService.GetLogsOfObra(idObra);
        Assert.True(logs.Count == 1);
    }
}
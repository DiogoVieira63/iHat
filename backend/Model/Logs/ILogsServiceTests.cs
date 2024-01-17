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

        var databaseSettings = config.GetSection("Database").Get<DatabaseSettings>();
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
        // Arrange
        var logsService = Setup();
        Assert.NotNull(logsService);

        var log1 = new Log("Alerta", DateTime.Today, "1", 1, "1", "Fall");
        var log2 = new Log("Grave", DateTime.Parse("03/01/2024"), "1", 2, "2", "Fall");
        
        // Act
        await logsService.Add(log1);
        await logsService.Add(log2);

        var logs = await logsService.GetLogsOfObra("1");
        // Assert
        Assert.NotNull(logs);
    }
}
using Xunit;
using Moq;
using Microsoft.Extensions.Options;
using iHat.Model.MensagensCapacete;
using iHat.MensagensCapacete.Values;

namespace iHat.Model.MessagesHelmet;

public class MensagemCapaceteServiceTests{

    private static IMensagemCapaceteService? Setup(){
        var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables() 
                .Build();
        var databaseSettings = config.GetSection("Database").Get<DatabaseSettings>();
            // Console.WriteLine($"Database Settings: {databaseSettings}");

        if(databaseSettings == null){
            return null;
        }
        IOptions<DatabaseSettings> options1 = Options.Create<DatabaseSettings>(databaseSettings);
            
        var logger = Mock.Of<ILogger<MensagemCapaceteService>>();
        var mensagemCapaceteService= new MensagemCapaceteService(options1, logger);
    
        return mensagemCapaceteService;
    }
        

    [Fact]
    public async void Test_AddMensagemCapacete(){
        var mensagem = new MensagemCapacete(1, "Grave", false, 36.5, 80, "Proximity", "Position", new Location(1.0, 1.0, 1.0), new Gases(1.0, 1.0));
        var mensagemCapaceteService = Setup();
        Assert.NotNull(mensagemCapaceteService);
        
        await mensagemCapaceteService.Add(mensagem);
        var listaMensagemCapacete = await mensagemCapaceteService.GetUltimosDadosDoCapacete(1);
        Assert.Equal(1, listaMensagemCapacete[0].NCapacete);
        Assert.Equal("Grave", listaMensagemCapacete[0].Type);
        Assert.Equal(false, listaMensagemCapacete[0].Fall);
        Assert.Equal(36.5, listaMensagemCapacete[0].BodyTemperature.Value);
        Assert.Equal(80, listaMensagemCapacete[0].Heartrate.Value);
        Assert.Equal("Proximity", listaMensagemCapacete[0].Proximity);
        Assert.Equal("Position", listaMensagemCapacete[0].Position);
        Assert.Equal(1.0, listaMensagemCapacete[0].Location.X);
        Assert.Equal(1.0, listaMensagemCapacete[0].Location.Y);
        Assert.Equal(1.0, listaMensagemCapacete[0].Location.Z);
        Assert.Equal(1.0, listaMensagemCapacete[0].Gases.Metano);
        Assert.Equal(1.0, listaMensagemCapacete[0].Gases.MonoxidoCarbono);
    }


    [Fact]
    public async void Test_GetLastLocation(){
        var mensagemCapaceteService = Setup();
        Assert.NotNull(mensagemCapaceteService);
        var location = await mensagemCapaceteService.GetLastLocation(1);
        Assert.Equal(1.0, location.X);
        Assert.Equal(1.0, location.Y);
        Assert.Equal(1.0, location.Z);
    }

}
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

        await mensagemCapaceteService!.Add(mensagem);
        /*
        var sortDefinition = Builders<MensagemCapacete>.Sort.Descending("timestamp");
        var listaMensagemCapacete = await _mensagemcapaceteCollection.Find(x => x.NCapacete == nCapacete).Sort(sortDefinition).ToListAsync();
        var moreRecentMensagens = listaMensagemCapacete.Take(20).ToList();
        return moreRecentMensagens; 
        */
        var moreRecentMensagens = await mensagemCapaceteService!.GetUltimosDadosDoCapacete(1);  
        Assert.Equal(mensagem, moreRecentMensagens[0]);
    }

    [Fact]
    public async void Test_GetUltimosDadosDoCapacete(){
        
    
    }

    [Fact]
    public async void Test_GetLastLocation(){

    }

}
using Xunit;
using Moq;
using Microsoft.Extensions.Options;

namespace iHat.Model.Obras;

public class ObrasServiceTests{

    private static IObrasService? Setup(){
        var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                 .AddEnvironmentVariables() 
                 .Build();

        var databaseSettings =  (DatabaseSettings?) config.GetValue(typeof(DatabaseSettings), "Database" );
        if(databaseSettings == null){
            return null;
        }
        IOptions<DatabaseSettings> options1 = Options.Create<DatabaseSettings>(databaseSettings);
        
        var logger = Mock.Of<ILogger<ObrasService>>();

        var obraService = new ObrasService(options1, logger);

        return obraService;
    }

    [Fact]
    public async void Test_AddObra(){
        var idResponsavel = 1;
        var nameObra = "Obra5";

        // Arrange
        var obraService = Setup();
        if(obraService == null)
            return;

        var allPreviousObras = await obraService.GetObrasOfResponsavel(idResponsavel);

        // Act
        await obraService.AddObra(nameObra, idResponsavel, new List<string>());

        // Assert [Se já existir um valor antes na lista, esta não deverá ter sido adicionada]
        var allAfterObras = await obraService.GetObrasOfResponsavel(idResponsavel);

        var expectedValue = allPreviousObras.Find(obra => obra.Name == nameObra) == null;

        var addedObra = allAfterObras.Find(obra => obra.Name == nameObra) != null;

        addedObra.Equals(expectedValue);
    }


    [Fact]
    public async void Test_AddObra_FailBecauseTheNameAlreadyExists(){
        var idResponsavel = 1;
        var nameObra = "Obra5";

        // Arrange
        var obraService = Setup();
        if(obraService == null)
            return;

        try{
            await obraService.AddObra(nameObra, idResponsavel, new List<string>());
        }
        catch(Exception){}

        // Act
        Action act = () => obraService.AddObra(nameObra, idResponsavel, new List<string>());
        
        //assert
        Exception exception = Assert.Throws<Exception>(act);
        //The thrown exception can be used for even more detailed assertions.
        Assert.Equal("Construction with this name already exists.", exception.Message);   
    }

    
    [Fact]
    // Acho que este teste não é preciso, porque a função GetObra consiste apenas numa chamada a uma função da biblioteca MongoDb.Driver 
    public async void Test_GetConstructionById(){
        var idResponsavel = 1;
    
        var obrasService = Setup();
        if(obrasService == null){
            return;
        }

        var allObras = await obrasService.GetObrasOfResponsavel(idResponsavel);
        if(allObras.Count > 0){
            var firstElement = allObras[0];
            if(firstElement == null){
                return;
            }
            var obra = await obrasService.GetConstructionById(firstElement.Id!);    

            obra.Equals(firstElement);
        }
    }
}
using Xunit;
using Moq;
using Microsoft.Extensions.Options;
using System.Security.AccessControl;

using iHat.Model;

namespace iHat.Model.Obras;

public class ObrasServiceTests{

    [Fact]
    public async void AddObraTest(){
        var idResponsavel = 1;
        var nameObra = "Obra5";

        // Arrange
        var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                 .AddEnvironmentVariables() 
                 .Build();

        var databaseSettings =  (DatabaseSettings?) config.GetValue(typeof(DatabaseSettings), "Database" );
        if(databaseSettings == null){
            return;
        }

        var logger = Mock.Of<ILogger<ObrasService>>();

        IOptions<DatabaseSettings> options1 = Options.Create<DatabaseSettings>(databaseSettings);

        var obraService = new ObrasService(options1, logger);
        
        var allPreviousObras = await obraService.GetObrasOfResponsavel(idResponsavel);

        // Act
        await obraService.AddObra(nameObra, idResponsavel, "", "Planeada");

        // Assert [Se já existir um valor antes na lista, esta não deverá ter sido adicionada]
        var allAfterObras = await obraService.GetObrasOfResponsavel(idResponsavel);

        var expectedValue = allPreviousObras.Find(obra => obra.Name == nameObra) == null;

        var addedObra = allAfterObras.Find(obra => obra.Name == nameObra) != null;

        addedObra.Equals(expectedValue);
    }



}
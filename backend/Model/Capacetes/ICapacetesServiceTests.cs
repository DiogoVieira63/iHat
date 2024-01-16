using Xunit;
using Moq;
using Microsoft.Extensions.Options;

namespace iHat.Model.Capacetes;

public class CapacetesServiceTests{
    
        private static ICapacetesService? Setup(){
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
            
            var logger = Mock.Of<ILogger<CapacetesService>>();
    
            var capaceteService = new CapacetesService(options1, logger);
    
            return capaceteService;
        }
    
    [Fact]
    public async void Test_AddCapacete(){
        var nCapacete = 3;
        // var status = "Livre";

        // Arrange
        var capaceteService = Setup();
        Assert.NotNull(capaceteService);
        // if(capaceteService == null)
        //     return;

        var allPreviousCapacetes = await capaceteService.GetAll();

        // Act
        await capaceteService.Add(nCapacete);

        // Assert [Se já existir um valor antes na lista, esta não deverá ter sido adicionada]
        var allAfterCapacetes = await capaceteService.GetAll();

        var expectedValue = allPreviousCapacetes.Find(capacete => capacete.NCapacete == nCapacete) == null;

        var addedCapacete = allAfterCapacetes.Find(capacete => capacete.NCapacete == nCapacete) != null;

        addedCapacete.Equals(expectedValue);
    }

    [Fact]
    public async void Test_AddCapacete_FailBecauseTheCapaceteAlreadyExists(){
        var nCapacete = 12;
        // var status = "Livre";

        // Arrange
        var capaceteService = Setup();
        Assert.NotNull(capaceteService);

        await capaceteService.Add(nCapacete);

        // Act
        Action act = () => capaceteService.Add(nCapacete);
        
        //assert
        Assert.Throws<Exception>(act);
    }

    [Fact]
    public async void Test_GetAll(){
        // Arrange
        var capaceteService = Setup();
        Assert.NotNull(capaceteService);

        // Act
        var allCapacetes = await capaceteService.GetAll();

        // Assert
        Assert.NotNull(allCapacetes);
    }

    [Fact]
    public async void Test_GetById(){
        var nCapacete = 1;
        // var status = "Livre";

        // Arrange
        var capaceteService = Setup();
        Assert.NotNull(capaceteService);

        // Act
        var capacete = await capaceteService.GetById(nCapacete);

        // Assert
        Assert.NotNull(capacete);
    }

    [Fact]
    public async void Test_GetById_FailBecauseTheCapaceteDoesNotExist(){
        var nCapacete = 3;
        // var status = "Livre";

        // Arrange
        var capaceteService = Setup();
        Assert.NotNull(capaceteService);

        // Act
        var capacete = await capaceteService.GetById(nCapacete);

        // verificar que o capacete nao existe
        Assert.Null(capacete);
    
    }

    [Fact]
    public async void Test_GetAllHelmetsFromList(){
        var listNCapacetes = new List<int>{1,2,3};

        // Arrange
        var capaceteService = Setup();
       Assert.NotNull(capaceteService);

        // Act
        var capacetes = await capaceteService.GetAllHelmetsFromList(listNCapacetes);

        // Assert
        Assert.NotNull(capacetes);
    }
    
    [Fact]
    public async void Test_GetAllHelmetsFromList_FailBecauseTheCapaceteDoesNotExist(){
        var listNCapacetes = new List<int>{1,2,3,4};

        // Arrange
        var capaceteService = Setup();
        Assert.NotNull(capaceteService);

        // Act
        var capacetes = await capaceteService.GetAllHelmetsFromList(listNCapacetes);

        // Assert
        Assert.NotNull(capacetes);
    }

    [Fact]
    public async void Test_CheckIfCapaceteExists(){
        var nCapacete = 1;
        // var status = "Livre";

        // Arrange
        var capaceteService = Setup();
       Assert.NotNull(capaceteService);

        // Act
        var capacete = await capaceteService.CheckIfCapaceteExists(nCapacete);

        // Assert
        Assert.True(capacete);
    }

    [Fact]
    public async void Test_CheckIfCapaceteExists_FailBecauseTheCapaceteDoesNotExist(){
        var nCapacete = 2;
        // var status = "Livre";

        // Arrange
        var capaceteService = Setup();
        Assert.NotNull(capaceteService);

        // Act
        var capacete = await capaceteService.CheckIfCapaceteExists(nCapacete);

        // Assert
        Assert.False(capacete);
    }

    [Fact]
    public async void Test_CheckIfHelmetIfBeingUsed(){
        var nCapacete = 1;
        // var status = "Livre";

        // Arrange
        var capaceteService = Setup();
        Assert.NotNull(capaceteService);

        // Act
        var capacete = await capaceteService.CheckIfHelmetIfBeingUsed(nCapacete);

        // Assert
        Assert.False(capacete);
    }

    [Fact]
    public async void Test_CheckIfHelmetIfBeingUsed_FailBecauseTheCapaceteDoesNotExist(){
        var nCapacete = 1;
        // var status = "Livre";

        // Arrange
        var capaceteService = Setup();
        Assert.NotNull(capaceteService);

        // Act
        var capacete = await capaceteService.CheckIfHelmetIfBeingUsed(nCapacete);

        // Assert
        Assert.False(capacete);
    }

    // [Fact]
    // public async void Test_UpdateCapaceteStatusToLivre(){
    //     var nCapacete = 1;
    //     var status = "Livre";

    //     // Arrange
    //     var capaceteService = Setup();
    //     if(capaceteService == null)
    //         return;

    //     // Act
    //     await capaceteService.UpdateCapaceteStatusToLivre(nCapacete);

    //     // Assert
    //     var capacete = await capaceteService.GetById(nCapacete);
    //     Assert.Equal(status, capacete?.Status);
    // }

    // [Fact]
    // public async void Test_UpdateCapaceteStatusToLivre_FailBecauseTheCapaceteDoesNotExist(){
    //     var nCapacete = 1;

    //     // Arrange
    //     var capaceteService = Setup();
    //     if(capaceteService == null)
    //         return;

    //     // Act
    //     Action act = () => capaceteService.UpdateCapaceteStatusToLivre(nCapacete);
        
    //     //assert
    //     Assert.Throws<Exception>(act);
    // }

    [Fact]
    public async void Test_UpdateCapaceteStatus(){
        var nCapacete = 1;
        var status = "Em uso";

        // Arrange
        var capaceteService = Setup();
        Assert.NotNull(capaceteService);

        // Act
        await capaceteService.UpdateCapaceteStatus(nCapacete, status);

        // Assert
        var capacete = await capaceteService.GetById(nCapacete);
        Assert.Equal(status, capacete?.Status);
    }

    [Fact]
    public async void Test_UpdateCapaceteStatus_FailBecauseTheCapaceteDoesNotExist(){
        var nCapacete = 1;
        var status = "Em uso";

        // Arrange
        var capaceteService = Setup();
        Assert.NotNull(capaceteService);

        // Act
        Action act = () => capaceteService.UpdateCapaceteStatus(nCapacete, status);
        
        //assert
        Assert.Throws<Exception>(act);
    }

    [Fact]
    public async void Test_GetFreeHelmets(){
        // Arrange
        var capaceteService = Setup();
        Assert.NotNull(capaceteService);

        // Act
        var capacetes = await capaceteService.GetFreeHelmets();

        // Assert
        Assert.NotNull(capacetes);
    }

    [Fact]
    public async void Test_AssociarTrabalhadorCapacete(){
        var nCapacete = 1;
        var idTrabalhador = "1";

        // Arrange
        var capaceteService = Setup();
        Assert.NotNull(capaceteService);

        // Act
        await capaceteService.AssociarTrabalhadorCapacete(nCapacete, idTrabalhador);

        // Assert
        var capacete = await capaceteService.GetById(nCapacete);
        Assert.Equal(idTrabalhador, capacete?.Trabalhador);
    }

    [Fact]
    public async void Test_AssociarTrabalhadorCapacete_FailBecauseTheCapaceteDoesNotExist(){
        var nCapacete = 1;
        var idTrabalhador = "1";

        // Arrange
        var capaceteService = Setup();
        Assert.NotNull(capaceteService);

        // Act
        Action act = () => capaceteService.AssociarTrabalhadorCapacete(nCapacete, idTrabalhador);
        
        //assert
        Assert.Throws<Exception>(act);
    }

    [Fact]
    public async void Test_AssociarTrabalhadorCapacete_FailBecauseTheCapaceteAlreadyHasATrabalhador(){
        var nCapacete = 1;
        var idTrabalhador = "1";

        // Arrange
        var capaceteService = Setup();
        Assert.NotNull(capaceteService);

        // Act
        await capaceteService.AssociarTrabalhadorCapacete(nCapacete, idTrabalhador);

        // Assert
        Action act = () => capaceteService.AssociarTrabalhadorCapacete(nCapacete, idTrabalhador);
        
        //assert
        Assert.Throws<Exception>(act);
    }

    [Fact]
    public async void Test_AssociarTrabalhadorCapacete_FailBecauseTheTrabalhadorDoesNotExist(){
        var nCapacete = 1;
        var idTrabalhador = "1";

        // Arrange
        var capaceteService = Setup();
        Assert.NotNull(capaceteService);

        // Act
        Action act = () => capaceteService.AssociarTrabalhadorCapacete(nCapacete, idTrabalhador);
        
        //assert
        Assert.Throws<Exception>(act);
    }

    [Fact]
    public async void Test_AssociarTrabalhadorCapacete_FailBecauseTheTrabalhadorIsAlreadyAssociatedToAnotherCapacete(){
        var nCapacete = 1;
        var idTrabalhador = "1";

        // Arrange
        var capaceteService = Setup();
        Assert.NotNull(capaceteService);

        // Act
        await capaceteService.AssociarTrabalhadorCapacete(nCapacete, idTrabalhador);

        // Assert
        Action act = () => capaceteService.AssociarTrabalhadorCapacete(nCapacete, idTrabalhador);
        
        //assert
        Assert.Throws<Exception>(act);
    }

// REVER
    [Fact]
    public async Task Test_DesassociarTrabalhadorCapacete() {
        int nCapacete = 2;
        string idTrabalhador = "1";
        // Arrange
        var capaceteService = Setup();
        Assert.NotNull(capaceteService);

        await capaceteService.AssociarTrabalhadorCapacete(nCapacete, idTrabalhador);
        // var capacete = await capaceteService.GetById(nCapacete);
        // Assert.Equal(idTrabalhador, capacete?.Trabalhador);
        // Act
        await capaceteService.DesassociarTrabalhadorCapacete(nCapacete, idTrabalhador);
        // Assert
        var capacete = await capaceteService.GetById(nCapacete);
        Assert.Null(capacete?.Trabalhador);

        
    }

}
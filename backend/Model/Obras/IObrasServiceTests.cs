using Xunit;
using Moq;
using Microsoft.Extensions.Options;
using iHat.Model.Zonas;
using Microsoft.Extensions.Configuration;

namespace iHat.Model.Obras;

public class ObrasServiceTests{

    private static IObrasService? Setup(){
        var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                 .AddEnvironmentVariables() 
                 .Build();

        var databaseSettings = config.GetSection("Database").Get<DatabaseSettings>();
        if(databaseSettings == null){
            Console.WriteLine("DatabaseSettings is null");

            return null;
        }
        IOptions<DatabaseSettings> options1 = Options.Create<DatabaseSettings>(databaseSettings);
        var logger = Mock.Of<ILogger<ObrasService>>();
        var obraService = new ObrasService(options1, logger);

        return obraService;
    }

    [Fact]
    public async void Test_AddObra(){
        var idResponsavel = 2;
        var nameObra = "Obra7";

        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);

        var allPreviousObras = await obraService.GetObrasOfResponsavel(idResponsavel);

        // Act
        await obraService.AddObra(nameObra, idResponsavel, new List<string>());

        // Assert [Se já existir um valor antes na lista, esta não deverá ter sido adicionada]
        var allAfterObras = await obraService.GetObrasOfResponsavel(idResponsavel);

        var expectedValue = allPreviousObras.Find(obra => obra.Nome == nameObra) == null;

        var addedObra = allAfterObras.Find(obra => obra.Nome == nameObra) != null;

        addedObra.Equals(expectedValue);
    }


    [Fact]
    public async void Test_AddObra_FailBecauseTheNameAlreadyExists(){
        var idResponsavel = 1;
        var nameObra = "Obra5";

        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);

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
        var idResponsavel = 2;
    
        var obrasService = Setup();
        Assert.NotNull(obrasService);

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

    [Fact]
    public async void Test_RemoveObraByIdAsync(){
        var idResponsavel1 = 1;
        var nameObra1 = "Obra8";

        var idResponsavel2 = 2;
        var nameObra2 = "Obra9";

        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);
        
        // Act - Adicionar uma Obra
        var obra1 = await obraService.AddObra(nameObra1, idResponsavel1, new List<string>());
        var obra2 = await obraService.AddObra(nameObra2, idResponsavel2, new List<string>());

        // Assert - Verificar se a Obra foi adicionada
        var allObras = await obraService.GetObrasOfResponsavel(idResponsavel1);
        var addedObra = allObras.Find(obra => obra.Nome == nameObra1) != null;
        addedObra.Equals(true);

        // Act - Remover a Obra
        await obraService.RemoveObraById(obra1);
        // Assert - Verificar se a Obra foi removida
        allObras = await obraService.GetObrasOfResponsavel(idResponsavel1);
        addedObra = allObras.Find(obra => obra.Nome == nameObra1) != null;
        addedObra.Equals(false);
    }

    [Fact]
    public async void Test_RemoveObraByIdAsync_FailBecauseTheObraDoesNotExist(){
        var idResponsavel = 1;
        var nameObra = "5Obra";

        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);

        // Act - Adicionar uma Obra
        var obra = await obraService.AddObra(nameObra, idResponsavel, new List<string>());

        // Assert - Verificar se a Obra foi adicionada
        var allObras = await obraService.GetObrasOfResponsavel(idResponsavel);
        var addedObra = allObras.Find(obra => obra.Nome == nameObra) != null;
        addedObra.Equals(true);

        // Act - Remover a Obra
        await obraService.RemoveObraById(obra);
        // Assert - Verificar se a Obra foi removida
        allObras = await obraService.GetObrasOfResponsavel(idResponsavel);
        addedObra = allObras.Find(obra => obra.Nome == nameObra) != null;
        addedObra.Equals(false);

        // Act - Remover a Obra
        Action act = () => obraService.RemoveObraById(obra);
        
        //assert
        Exception exception = Assert.Throws<Exception>(act);
        //The thrown exception can be used for even more detailed assertions.
        Assert.Equal("Construction with this id does not exist.", exception.Message);   
    }

    [Fact]
    public async void Test_AlteraEstadoObra(){
        var idResponsavel = 1;
        var nameObra = "Obra10";

        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);

        // Act - Adicionar uma Obra
        var obra = await obraService.AddObra(nameObra, idResponsavel, new List<string>());

        // Assert - Verificar se a Obra foi adicionada
        var allObras = await obraService.GetObrasOfResponsavel(idResponsavel);
        var addedObra = allObras.Find(obra => obra.Nome == nameObra) != null;
        addedObra.Equals(true);

        // Act - Alterar o estado da Obra
        await obraService.UpdateEstadoObra(obra, "Cancelada");
        // Assert - Verificar se o estado da Obra foi alterado
        allObras = await obraService.GetObrasOfResponsavel(idResponsavel);
        var obraAlterada = allObras.Find(obra => obra.Nome == nameObra && obra.Status == "Cancelada") != null;
        obraAlterada.Equals(true);
    }

    [Fact]
    public async void Test_AlteraEstadoObra_FailBecauseTheObraDoesNotExist(){
        var idResponsavel = 1;
        var nameObra = "Obra555";

        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);

        // Act - Adicionar uma Obra
        var obra = await obraService.AddObra(nameObra, idResponsavel, new List<string>());

        // Assert - Verificar se a Obra foi adicionada
        var allObras = await obraService.GetObrasOfResponsavel(idResponsavel);
        var addedObra = allObras.Find(obra => obra.Nome == nameObra) != null;
        addedObra.Equals(true);

        // Act - Alterar o estado da Obra
        await obraService.UpdateEstadoObra(obra, "Cancelada");
        // Assert - Verificar se o estado da Obra foi alterado
        allObras = await obraService.GetObrasOfResponsavel(idResponsavel);
        var obraAlterada = allObras.Find(obra => obra.Nome == nameObra && obra.Status == "Cancelada") != null;
        obraAlterada.Equals(true);

        // Act - Alterar o estado da Obra
        Action act = () => obraService.UpdateEstadoObra(obra, "Cancelada");
        
        //assert
        Exception exception = Assert.Throws<Exception>(act);
        //The thrown exception can be used for even more detailed assertions.
        Assert.Equal("Construction with this id does not exist.", exception.Message);   
    }

    [Fact]
    public async void Test_UpdateNomeObra(){
        var idResponsavel = 1;
        var nameObra = "Obra_n5";
        var newNameObra = "Obra_5";

        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);

        // Act - Adicionar uma Obra
        var obra = await obraService.AddObra(nameObra, idResponsavel, new List<string>());

        // Assert - Verificar se a Obra foi adicionada
        var allObras = await obraService.GetObrasOfResponsavel(idResponsavel);
        var addedObra = allObras.Find(obra => obra.Nome == nameObra) != null;
        addedObra.Equals(true);

        // Act - Alterar o nome da Obra
        await obraService.UpdateNomeObra(obra, newNameObra);
        // Assert - Verificar se o nome da Obra foi alterado
        allObras = await obraService.GetObrasOfResponsavel(idResponsavel);
        var obraAlterada = allObras.Find(obra => obra.Nome == newNameObra) != null;
        obraAlterada.Equals(true);
    }

    [Fact]
    public async void Test_UpdateNomeObra_FailBecauseTheObraDoesNotExist(){
        var idResponsavel = 1;
        var nameObra = "Obra155";
        var newNameObra = "Obra_155";

        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);

        // Act - Adicionar uma Obra
        var obra = await obraService.AddObra(nameObra, idResponsavel, new List<string>());

        // Assert - Verificar se a Obra foi adicionada
        var allObras = await obraService.GetObrasOfResponsavel(idResponsavel);
        var addedObra = allObras.Find(obra => obra.Nome == nameObra) != null;
        addedObra.Equals(true);

        // Act - Alterar o nome da Obra
        await obraService.UpdateNomeObra(obra, newNameObra);
        // Assert - Verificar se o nome da Obra foi alterado
        allObras = await obraService.GetObrasOfResponsavel(idResponsavel);
        var obraAlterada = allObras.Find(obra => obra.Nome == newNameObra) != null;
        obraAlterada.Equals(true);

        // Act - Alterar o nome da Obra
        Action act = () => obraService.UpdateNomeObra(obra, newNameObra);
        
        //assert
        Exception exception = Assert.Throws<Exception>(act);
        //The thrown exception can be used for even more detailed assertions.
        Assert.Equal("Construction with this id does not exist.", exception.Message);   
    }

    [Fact]
    public async void Test_GetObraWithCapaceteId(){
        var idResponsavel = 1;
        var nameObra = "Obra12";
        var idCapacete = 1;

        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);

        // Act - Adicionar uma Obra
        var obra = await obraService.AddObra(nameObra, idResponsavel, new List<string>());
        await obraService.AddCapaceteToObra(idCapacete, obra);

        // Assert - Verificar se a Obra foi adicionada
        var obraWithCapacete = await obraService.GetObraWithCapaceteId(idCapacete);
        obraWithCapacete.Equals(obra);
    }

    [Fact]
    public async void Test_GetObraWithCapaceteId_FailBecauseTheObraDoesNotExist(){
        var idResponsavel = 1;
        var nameObra = "Obra53";
        var idCapacete = 1;

        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);

        // Act - Adicionar uma Obra
        var obra = await obraService.AddObra(nameObra, idResponsavel, new List<string>());
        await obraService.AddCapaceteToObra(idCapacete, obra);

        // Assert - Verificar se a Obra foi adicionada
        var obraWithCapacete = await obraService.GetObraWithCapaceteId(idCapacete);
        obraWithCapacete.Equals(obra);

        // Act - Adicionar uma Obra
        Action act = () => obraService.GetObraWithCapaceteId(idCapacete);
        
        //assert
        Exception exception = Assert.Throws<Exception>(act);
        //The thrown exception can be used for even more detailed assertions.
        Assert.Equal("Construction with this capacete does not exist.", exception.Message);   
    }

    [Fact]
    public async void Test_GetAllCapacetesOfObra(){
        var idResponsavel = 1;
        var nameObra = "Obra_54";
        var idCapacete1 = 1;
        var idCapacete2 = 2;

        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);

        // Act - Adicionar uma Obra
        var obra = await obraService.AddObra(nameObra, idResponsavel, new List<string>());
        await obraService.AddCapaceteToObra(idCapacete1, obra);
        await obraService.AddCapaceteToObra(idCapacete2, obra);

        // Assert - Verificar se a Obra foi adicionada
        var allCapacetes = await obraService.GetAllCapacetesOfObra(obra);
        var capacete1 = allCapacetes.Find(capacete => capacete == idCapacete1) != null;
        var capacete2 = allCapacetes.Find(capacete => capacete == idCapacete2) != null;
        capacete1.Equals(true);
        capacete2.Equals(true);
    }

    [Fact]
    public async void Test_GetAllCapacetesOfObra_FailBecauseTheObraDoesNotExist(){
        var idResponsavel = 1;
        var nameObra = "Obra63";
        var idCapacete1 = 1;
        var idCapacete2 = 2;

        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);

        // Act - Adicionar uma Obra
        var obra = await obraService.AddObra(nameObra, idResponsavel, new List<string>());
        await obraService.AddCapaceteToObra(idCapacete1, obra);
        await obraService.AddCapaceteToObra(idCapacete2, obra);

        // Assert - Verificar se a Obra foi adicionada
        var allCapacetes = await obraService.GetAllCapacetesOfObra(obra);
        var capacete1 = allCapacetes.Find(capacete => capacete == idCapacete1) != null;
        var capacete2 = allCapacetes.Find(capacete => capacete == idCapacete2) != null;
        capacete1.Equals(true);
        capacete2.Equals(true);

        // Act - Adicionar uma Obra
        Action act = () => obraService.GetAllCapacetesOfObra(obra);
        
        //assert
        Exception exception = Assert.Throws<Exception>(act);
        //The thrown exception can be used for even more detailed assertions.
        Assert.Equal("Construction with this id does not exist.", exception.Message);   
    }

    [Fact]
    public async void Test_CheckIfObraExists(){
        var idResponsavel = 1;
        var nameObra = "Obra59";

        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);

        // Act - Adicionar uma Obra
        await obraService.AddObra(nameObra, idResponsavel, new List<string>());

        // Assert - Verificar se a Obra foi adicionada
        var allObras = await obraService.GetObrasOfResponsavel(idResponsavel);
        var addedObra = allObras.Find(obra => obra.Nome == nameObra) != null;
        addedObra.Equals(true);

        // Act - Verificar se a Obra existe
        var obraExists = await obraService.CheckIfObraExists(allObras[0].Id!);
        obraExists.Equals(true);
    }

    [Fact]
    public async void Test_CheckIfObraExists_FailBecauseTheObraDoesNotExist(){
        var idResponsavel = 1;
        var nameObra = "Obra33";

        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);

        // Act - Adicionar uma Obra
        await obraService.AddObra(nameObra, idResponsavel, new List<string>());

        // Assert - Verificar se a Obra foi adicionada
        var allObras = await obraService.GetObrasOfResponsavel(idResponsavel);
        var addedObra = allObras.Find(obra => obra.Nome == nameObra) != null;
        addedObra.Equals(true);

        // Act - Verificar se a Obra existe
        var obraExists = await obraService.CheckIfObraExists(allObras[0].Id!);
        obraExists.Equals(true);

        // Act - Verificar se a Obra existe
        Action act = () => obraService.CheckIfObraExists(allObras[0].Id!);
        
        //assert
        Exception exception = Assert.Throws<Exception>(act);
        //The thrown exception can be used for even more detailed assertions.
        Assert.Equal("Construction with this id does not exist.", exception.Message);   
    }

    [Fact]
    public async void Test_DeleteCapaceteToObra(){
        var idResponsavel = 1;
        var nameObra = "Obra81";
        var idCapacete1 = 1;
        var idCapacete2 = 2;
        var capacetesInicialmenteNaObra = new List<int>{idCapacete1, idCapacete2};
        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);
        
        // Act - Adicionar uma Obra
        var obra = await obraService.AddObra(nameObra, idResponsavel, new List<string>());
        await obraService.AddCapaceteToObra(idCapacete1, obra);
        await obraService.AddCapaceteToObra(idCapacete2, obra);
        // Assert - Remover um Capacete da Obra
        await obraService.RemoveCapaceteFromObra(idCapacete1, obra);
        // Assert - Verificar se o Capacete foi removido da Obra
        var allCapacetes = await obraService.GetAllCapacetesOfObra(obra);
        var capacete1 = allCapacetes.Find(capacete => capacete == idCapacete1) != null;
        var capacete2 = allCapacetes.Find(capacete => capacete == idCapacete2) != null;
        capacete1.Equals(false);
        capacete2.Equals(true);

    }

    [Fact]
    public async void Test_DeleteCapaceteToObra_FailBecauseTheObraDoesNotExist(){
        var idResponsavel = 1;
        var nameObra = "Obra11111";
        var idCapacete1 = 1;
        var idCapacete2 = 2;
        var capacetesInicialmenteNaObra = new List<int>{idCapacete1, idCapacete2};
        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);
        
        // Act - Adicionar uma Obra
        var obra = await obraService.AddObra(nameObra, idResponsavel, new List<string>());
        await obraService.AddCapaceteToObra(idCapacete1, obra);
        await obraService.AddCapaceteToObra(idCapacete2, obra);
        // Assert - Remover um Capacete da Obra
        await obraService.RemoveCapaceteFromObra(idCapacete1, obra);
        // Assert - Verificar se o Capacete foi removido da Obra
        var allCapacetes = await obraService.GetAllCapacetesOfObra(obra);
        var capacete1 = allCapacetes.Find(capacete => capacete == idCapacete1) != null;
        var capacete2 = allCapacetes.Find(capacete => capacete == idCapacete2) != null;
        capacete1.Equals(false);
        capacete2.Equals(true);

        // Act - Remover um Capacete da Obra
        Action act = () => obraService.RemoveCapaceteFromObra(idCapacete1, obra);
        
        //assert
        Exception exception = Assert.Throws<Exception>(act);
        //The thrown exception can be used for even more detailed assertions.
        Assert.Equal("Construction with this id does not exist.", exception.Message);   
    }

    [Fact]
    public async void Test_UpdateZonasRiscoObra(){
        var idResponsavel = 1;
        var nameObra = "Obra625";
        var zonasRisco = new List<ZonasRisco>{
            new ZonasRisco("a1b2c3d4e5f6a1b2c3d4e5f6"){
                Points = new List<Point>{
                    new Point(992.9577606666666, 709.2555433333333),
                    new Point(947.823317, 947.823317),
                    new Point(1373.3766429999998, 702.8077656666666)
                }
            }
        };
        var idMapa = "a1b2c3d4e5f6a1b2c3d4e5f6";

        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);
        
        // Act - Adicionar uma Obra
        var obra = await obraService.AddObra(nameObra, idResponsavel, new List<string>{idMapa});
        // Assert - Alterar as Zonas de Risco da Obra
        await obraService.UpdateZonasRiscoObra(obra, idMapa);

        // Assert - Verificar se as Zonas de Risco foram alteradas
        var allMapas = await obraService.GetConstructionById(obra);
        var mapa = allMapas.Mapa.Find(mapa => mapa == idMapa) != null;
        mapa.Equals(true);

        // var allZonasRisco = await obraService.GetConstructionById(obra);
        // var zonasRisco1 = allZonasRisco.ZonasRisco.Find(zona => zona.IdZona == zonasRisco[0].IdZona) != null;
        // zonasRisco1.Equals(true);
     }

    [Fact]
    public async void Test_AddListaMapaToObra(){
        var idResponsavel = 1;
        var nameObra = "Obra135";
        var mapas = new List<string>{"a1b2c3d4e5f6a1b2c3d4e5f6", "a1b2c3d4e5f6a1b2c3d4e5f7"};

        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);
        
        // Act - Adicionar uma Obra
        var obra = await obraService.AddObra(nameObra, idResponsavel, new List<string>());
        // Assert - Adicionar uma lista de mapas à Obra
        var mapasAdicionados = await obraService.AddListaMapaToObra(obra, mapas);
        // Assert - Verificar se os mapas foram adicionados à Obra
        var allMapas = await obraService.GetConstructionById(obra);
        var mapa1 = allMapas.Mapa.Find(mapa => mapa == mapas[0]) != null;
        var mapa2 = allMapas.Mapa.Find(mapa => mapa == mapas[1]) != null;
        mapa1.Equals(true);
        mapa2.Equals(true);
    }

    [Fact]
    public async void Test_AddListaMapaToObra_FailBecauseTheObraDoesNotExist(){
        var idResponsavel = 1;
        var nameObra = "Obra4321568";
        var mapas = new List<string>{"Mapa1", "Mapa2"};

        // Arrange
        var obraService = Setup();
        Assert.NotNull(obraService);
        
        // Act - Adicionar uma Obra
        var obra = await obraService.AddObra(nameObra, idResponsavel, new List<string>());
        // Assert - Adicionar uma lista de mapas à Obra
        var mapasAdicionados = await obraService.AddListaMapaToObra(obra, mapas);
        // Assert - Verificar se os mapas foram adicionados à Obra
        var allMapas = await obraService.GetConstructionById(obra);
        var mapa1 = allMapas.Mapa.Find(mapa => mapa == mapas[0]) != null;
        var mapa2 = allMapas.Mapa.Find(mapa => mapa == mapas[1]) != null;
        mapa1.Equals(true);
        mapa2.Equals(true);

        // Act - Adicionar uma lista de mapas à Obra
        Action act = () => obraService.AddListaMapaToObra(obra, mapas);
        
        //assert
        Exception exception = Assert.Throws<Exception>(act);
        //The thrown exception can be used for even more detailed assertions.
        Assert.Equal("Construction with this id does not exist.", exception.Message);   
    }
}
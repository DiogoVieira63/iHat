using Xunit;
using Moq;
using Microsoft.Extensions.Options;

namespace iHat.Model.Mapas;

public class MapaServiceTests{

    private static IMapaService? Setup(){
        var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                 .AddEnvironmentVariables() 
                 .Build();

        var databaseSettings =  (DatabaseSettings?) config.GetValue(typeof(DatabaseSettings), "Database" );
        if(databaseSettings == null){
            return null;
        }
        IOptions<DatabaseSettings> options1 = Options.Create<DatabaseSettings>(databaseSettings);
        
        var logger = Mock.Of<ILogger<MapaService>>();

        var mapaService = new MapaService(options1, logger);

        return mapaService;
    }

    [Fact]
    public async void Test_AddMapa(){
        var nameMapa = "Mapa5";
        var svg = "svg";
        var floor = 1;

        // Arrange
        var mapaService = Setup();
        if (mapaService == null)
            return;

        // Act
        var idMapa = await mapaService.Add(nameMapa, svg, floor);

        // Assert [Se já existir um valor antes na lista, esta não deverá ter sido adicionada]
        var mapa = idMapa != null ? await mapaService.GetMapaById(idMapa) : null;

        var expectedValue = mapa != null;

        expectedValue.Equals(true);
    }
    
    
    [Fact]
    public async void Test_GetZonasdeRisco(){
        var nameMapa = "Mapa5";
        var svg = "svg";
        var floor = 1;

        // Arrange
        var mapaService = Setup();
        if (mapaService == null)
            return;

        // Act
        var idMapa = await mapaService.Add(nameMapa, svg, floor);

        // Assert [Se já existir um valor antes na lista, esta não deverá ter sido adicionada]
        if (idMapa != null)
        {
            var zonasDeRisco = await mapaService.GetZonasdeRisco(idMapa);
            Assert.NotNull(zonasDeRisco);
        }
        
    }

    [Fact]
    public async void Test_RemoveMapas(){
        var nameMapa1 = "Mapa5";
        var svg1 = "svg";
        var floor1 = 1;

        var nameMapa2 = "Mapa6";
        var svg2 = "svg";
        var floor2 = 1;


        // Arrange
        var mapaService = Setup();
        if (mapaService == null)
            return;

        // Act
        var idMapa1 = await mapaService.Add(nameMapa1, svg1, floor1);
        var idMapa2 = await mapaService.Add(nameMapa2, svg2, floor2);

        // Assert [Se já existir um valor antes na lista, esta não deverá ter sido adicionada]
        if (idMapa1 != null && idMapa2 != null)
        {
            await mapaService.RemoveMapas(new List<string>(){idMapa1});
            var mapa1 = await mapaService.GetMapaById(idMapa1);
            var mapa2 = await mapaService.GetMapaById(idMapa2);
            Assert.Null(mapa1);
            Assert.Null(mapa2);
        }
    }


    // Task UpdateFloorNumber(string id, int newFloorNumber);
    [Fact]
    public async void Test_UpdateFloorNumber(){
        var nameMapa = "Mapa5";
        var svg = "svg";
        var floor = 1;

        // Arrange
        var mapaService = Setup();
        if (mapaService == null)
            return;

        // Act
        var idMapa = await mapaService.Add(nameMapa, svg, floor);

        // Assert [Se já existir um valor antes na lista, esta não deverá ter sido adicionada]
        if (idMapa != null)
        {
            await mapaService.UpdateFloorNumber(idMapa, 2);
            var mapa = await mapaService.GetMapaById(idMapa);
            Assert.Equal(2, mapa?.Floor);
        }
        
    }
    





    



    
}
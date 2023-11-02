using iHat.Model.Obras;
using MongoDB.Bson.Serialization.Conventions;

namespace iHat.Model.iHatFacade;

public class iHatFacade: IiHatFacade{

    private readonly IObrasService iobras;

    public iHatFacade(IObrasService obrasService){
        iobras = obrasService;
    }

    public async Task NewConstruction(string name, string mapa, string status){

        // TO DO:
        // Obter o id do responsável que realizou o pedido do post
        var idResponsavel = 1;

        // Guarda na Base
        try{
            await iobras.AddObra(name, idResponsavel, mapa, status); 
        }
        catch(Exception e){
            throw new Exception(e.Message);
        }
    }


    public async Task<List<Obra>?> GetObras(int idResponsavel){

        var obras = await iobras.GetObrasOfResponsavel(idResponsavel);

        if(obras == null){
            Console.WriteLine("[iHatFacade] Lista de obras vazia.");
        }

        return obras;
    }

    public async Task<Obra> GetConstructionById(string idObra){
        return await iobras.GetConstructionById(idObra);
    }
}
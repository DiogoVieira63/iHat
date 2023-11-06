using iHat.Model.Obras;
using iHat.Model.Capacetes;

namespace iHat.Model.iHatFacade;

public class iHatFacade: IiHatFacade{

    private readonly IObrasService iobras;
    private readonly ICapacetesFacade icapacetes;

    public iHatFacade(IObrasService obrasService, ICapacetesFacade capacetesFacades){
        iobras = obrasService;
        icapacetes = capacetesFacades;
    }

    // public async Task NewConstruction(string name){

    //     // TO DO:
    //     // Obter o id do responsável que realizou o pedido do post
    //     var idResponsavel = 1;

    //     // Guarda na Base
    //     try{
    //         await iobras.AddObra(name, idResponsavel); 
    //     }
    //     catch(Exception e){
    //         throw new Exception(e.Message);
    //     }
    // }


    public async Task<List<Obra>> GetObras(int idResponsavel){

        var obras = await iobras.GetObrasOfResponsavel(idResponsavel);

        if(obras == null){
            Console.WriteLine("[iHatFacade] Lista de obras vazia.");
        }

        return obras;
    }

    public async Task AddHelmet(Capacete capacete){
        await icapacetes.Add(capacete);
    }

    public async Task<List<Capacete>> GetAll(){
        return await icapacetes.GetAll();
    }

    public async Task<Capacete> GetCapacete(string id){
        return await icapacetes.GetById(id);
    }

    public async Task<List<Capacete>> GetAllCapacetesdaObra(string idObra){
        return await icapacetes.GetAllCapacetesdaObra(idObra);
    }

    public async Task DeleteCapaceteToObra(string id, string idObra){
        await icapacetes.DeleteCapaceteToObra(id, idObra);
    }

    public async Task AddCapaceteToObra(string idCapacete, string idObra){
        await icapacetes.AddCapaceteToObra(idCapacete, idObra);
    }



}
using iHat.Model.Obras;
using iHat.Model.Capacetes;

namespace iHat.Model.iHatFacade;

public class iHatFacade: IiHatFacade{

    private readonly IObrasService iobras;
    private readonly ICapacetesFacade icapacetes;

    public iHatFacade(IObrasService obrasService){
        iobras = obrasService;
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

    public async Task<Capacete> GetCapacete(int id){
        return await icapacetes.GetById(id);
    }

    public async Task Delete(int id){
        await icapacetes.Delete(id);
    }

    public async Task AddCapaceteToObra(int idCapacete, string idObra){
        await icapacetes.AddCapaceteToObra(idCapacete, idObra);
    }



}
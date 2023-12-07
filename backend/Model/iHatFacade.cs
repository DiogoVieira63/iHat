using iHat.Model.Obras;
using iHat.Model.Capacetes;
using iHat.Model.Logs;

namespace iHat.Model.iHatFacade;

public class iHatFacade: IiHatFacade{

    private readonly IObrasService iobras;
    private readonly ICapacetesService icapacetes;
    private readonly ILogsService ilogs;

    public iHatFacade(IObrasService obrasService, ICapacetesService capacetesService, ILogsService logsService){
        iobras = obrasService;
        icapacetes = capacetesService;
        ilogs = logsService;
    }

    public async Task NewConstruction(string name, string mapa, string status){

        // TO DO:
        // Obter o id do responsável que realizou o pedido do post
        var idResponsavel = 1;

        await iobras.AddObra(name, idResponsavel, mapa, status); 
    }

    public async Task<List<Obra>?> GetObras(int idResponsavel){

        var obras = await iobras.GetObrasOfResponsavel(idResponsavel);

        if(obras == null){
            Console.WriteLine("[iHatFacade] Lista de obras vazia.");
        }

        return obras;
    }
    public async Task RemoveObraById(string obraId){

        await iobras.RemoveObraByIdAsync(obraId);
    }

    public async Task<Obra> GetConstructionById(string idObra){
        return await iobras.GetConstructionById(idObra);
    }


    public async Task<List<Capacete>> GetAllCapacetesdaObra(string idObra){
        var listaNCapacetes = await iobras.GetAllCapacetesOfObra(idObra);
        return await icapacetes.GetAllHelmetsFromList(listaNCapacetes);
    }

    public async Task DeleteCapaceteToObra(int nCapacete, string idObra){
        var existsCapacete = await icapacetes.CheckIfCapaceteExists(nCapacete);
        if(!existsCapacete)
            throw new Exception("Capacete não encontrado.");

        var capaceteIsBeingUsed = await icapacetes.CheckIfHelmetIfBeingUsed(nCapacete);
        if(!capaceteIsBeingUsed)
            throw new Exception("Capacete não pode ser removido da obra, pois não está em uso.");

        await iobras.DeleteCapaceteToObra(nCapacete, idObra);

        await icapacetes.UpdateCapaceteStatusToLivre(nCapacete);

    }

    // Talvez esta função devesse ser considerada uma zona critica, uma vez que estas funções deveriam ser realizadas uma a seguir às outras
    public async Task AddCapaceteToObra(int nCapacete, string idObra){
        var existsObra = await iobras.CheckIfObraExists(idObra);
        if(existsObra){
            // if the helmet doesn't exist, this function will return and exception and stop
            await icapacetes.AddCapaceteToObra(nCapacete);
            await iobras.AddCapaceteToObra(nCapacete, idObra);
        }
    }

    public async void AlteraEstadoObra(string id, string estado){
        iobras.AlteraEstadoObra(id, estado);
    }

    public async Task UpdateNomeObra(string idObra, string nome){
        await iobras.UpdateNomeObra(idObra, nome);
    }

    public async Task<List<Log>>  GetLogs(string idObra){
        return await ilogs.GetLogsOfObra(idObra);
    }

    public async Task AddLogs(Log logs){
        await ilogs.Add(logs);
    }


    public async Task ChangeStatusCapacete(int nCapacete, string newStatus){
        icapacetes.UpdateCapaceteStatus(nCapacete, newStatus);
    }



    // VERIFICADAS CAPACETES

    public async Task<List<Capacete>> GetAllCapacetes(){
        return await icapacetes.GetAll();
    }

    public async Task<Capacete?> GetCapacete(int nCapacete){
        return await icapacetes.GetById(nCapacete);
    }

    public async Task AddCapacete(int nCapacete){
        await icapacetes.Add(nCapacete);
    }



    // VERIFICAS OBRAS




}
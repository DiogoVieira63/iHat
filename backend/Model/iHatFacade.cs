using iHat.Model.Obras;
using iHat.Model.Capacetes;
using iHat.Model.Logs;
using System.IO.Compression;
using iHat.Model.Mapas;
using iHat.Model.Zonas;
using iHat.Model.MensagensCapacete;
using iHat.MensagensCapacete.Values;

namespace iHat.Model.iHatFacade;

public class iHatFacade: IiHatFacade {

    private readonly IObrasService iobras;
    private readonly ICapacetesService icapacetes;
    private readonly ILogsService ilogs;
    private readonly IMapaService imapas;
    private readonly IMensagemCapaceteService _mensagemCapaceteService;
    private readonly ILogger<iHatFacade> _logger;
    
    public iHatFacade(IObrasService obrasService, ICapacetesService capacetesService, ILogsService logsService, IMapaService mapasService, IMensagemCapaceteService mensagemCapaceteService, ILogger<iHatFacade> logger){
        iobras = obrasService;
        icapacetes = capacetesService;
        ilogs = logsService;
        imapas = mapasService;
        _mensagemCapaceteService = mensagemCapaceteService;
        _logger = logger;
    }


    public async Task<Dictionary<string, string>> requestHTTP(IFormFile mapaFile){
        // Request to python service "model2SVG"
        var listaSvg = new Dictionary<string, string>();

        // POST request to http://127.0.0.1:5000/ifc2sv {"ifc_file": "FILE"}
        using (HttpClient client = new HttpClient())
        using (MultipartFormDataContent content = new MultipartFormDataContent())
        {
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                mapaFile.CopyTo(ms);
                fileBytes = ms.ToArray();
            }
            ByteArrayContent fileContent = new ByteArrayContent(fileBytes);
            content.Add(fileContent, "ifc_file", "ifc_file"); // "file" is the name of the parameter expected by the server

            HttpResponseMessage response = await client.PostAsync("http://127.0.0.1:5000/ifc2svg", content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                throw new Exception("Unable to connect to python server");
            }

            var zipBytes = await response.Content.ReadAsByteArrayAsync();

            using (MemoryStream zipStream = new MemoryStream(zipBytes))
            using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read))
            {
                // Extract each entry in the zip archive
                foreach (ZipArchiveEntry entry in zipArchive.Entries)
                {
                    // Lê os bytes do arquivo
                    using (Stream entryStream = entry.Open())
                    using (StreamReader reader = new StreamReader(entryStream))
                    {
                        string contentFile = reader.ReadToEnd();
                        listaSvg.Add(entry.Name, contentFile);
                    }
                }
            }                
        } 

        return listaSvg; 
    }


    public async Task<string?> NewConstruction(string name, IFormFile? mapa, int idResponsavel){

        var listaSvgDBIds = new List<string>();
        if(mapa != null && mapa.Length != 0){

            var listaSvg = await requestHTTP(mapa);                   

            var number = 0;
            foreach(var svg in listaSvg){
                // new mapa value added to the db
                var ids = await imapas.Add(svg.Key, svg.Value, number);
                if(ids != null)
                    listaSvgDBIds.Add(ids);
                number++;
            }

        }
        var id = await iobras.AddObra(name, idResponsavel, listaSvgDBIds); 
        return id;
    }




    public async Task<List<Obra>> GetObras(int idResponsavel){
        var obras = await iobras.GetObrasOfResponsavel(idResponsavel);
        return obras;
    }

    public async Task<Obra?> GetConstructionById(string idObra){
        return await iobras.GetConstructionById(idObra);
    }

    public async Task<List<Capacete>> GetAllCapacetesdaObra(string idObra){
        var listaNCapacetes = await iobras.GetAllCapacetesOfObra(idObra);
        return await icapacetes.GetAllHelmetsFromList(listaNCapacetes);
    }

    public async Task RemoveObraById(string obraId){
        // Obtem a lista dos capacetes da Obra
        var capacetes = await GetAllCapacetesdaObra(obraId);

        // Remove a obra do sistema
        await iobras.RemoveObraById(obraId);

        // Remove a obra do Capacete
        foreach(var capacete in capacetes){
            await icapacetes.RemoveCapaceteFromObra(capacete.Numero, obraId);
        }
    }

    public async Task RemoveCapaceteFromObra(int nCapacete, string idObra){
        var capaceteIsInObra = await icapacetes.CheckIfCapaceteIsInObra(nCapacete, idObra);
        if(!capaceteIsInObra)
            throw new Exception("Capacete não pode ser removido da obra, pois não está a ser usado na obra.");

        var capaceteIsInUsed = await icapacetes.CheckIfCapaceteIsBeingUsed(nCapacete);
        if(capaceteIsInUsed)
            throw new Exception("Capacete está a ser utilizado e não pode ser removida da obra.");

        await iobras.RemoveCapaceteFromObra(nCapacete, idObra);
        await icapacetes.RemoveCapaceteFromObra(nCapacete, idObra);
    }

    public async Task AddCapaceteToObra(int nCapacete, string idObra){
        var existsObra = await iobras.CheckIfObraExists(idObra);
        if(existsObra){
            await icapacetes.AddCapaceteToObra(nCapacete, idObra);
            await iobras.AddCapaceteToObra(nCapacete, idObra);
        }
    }
    
    public async Task UpdateEstadoObra(string id, string estado){
        await iobras.UpdateEstadoObra(id, estado);
        
        if(estado == "Finalizada" || estado == "Cancelada"){
            var capacetes = await GetAllCapacetesdaObra(id);
            foreach(var capacete in capacetes){
                await icapacetes.RemoveCapaceteFromObra(capacete.Numero, id);
            }
        }
    }
    
    public async Task UpdateNomeObra(string idObra, string nome){
        await iobras.UpdateNomeObra(idObra, nome);
    }

    public async Task<List<Capacete>> GetAllCapacetes(){
        return await icapacetes.GetAll();
    }

    public async Task<Capacete?> GetCapacete(int nCapacete){
        return await icapacetes.GetById(nCapacete);
    }

    public async Task<List<Capacete>> GetFreeHelmets(){
        return await icapacetes.GetFreeHelmets();
    }

    public async Task UpdateCapaceteStatusFromToNaoOperacional(int nCapacete, string newStatus){
        await icapacetes.UpdateCapaceteStatus(nCapacete, newStatus);
    }

    public async Task AddCapacete(int nCapacete){
        await icapacetes.Add(nCapacete);
    }

    public async Task<List<MensagemCapacete>> GetUltimosDadosDoCapacete(int nCapacete){
        return await _mensagemCapaceteService.GetUltimosDadosDoCapacete(nCapacete);
    }

    public async Task<Dictionary<int, Location>> GetLastLocationCapacetesObra(string obraId){
        
        var listaCapacetes = await iobras.GetAllCapacetesOfObra(obraId);
        var allCapacetesLocation = new Dictionary<int, Location>();
        
        foreach(var id in listaCapacetes){
            var loc = await _mensagemCapaceteService.GetLastLocation(id);
            if (loc != null)
                allCapacetesLocation.Add(id, loc);
        }
        return allCapacetesLocation;
    }

    public async Task<List<Mapa>> GetMapasFromList(List<string> listaMapasIds){
        var results = new List<Mapa>();
        
        foreach(string id in listaMapasIds){
            var mapa = await imapas.GetMapaById(id);
            if(mapa != null)
                results.Add(mapa);
        }

        return results;
    }

    public async Task UpdateZonasRiscoObra(string idObra, string idMapa, List<ZonasRisco> zonas){
        var canUpdateZonasRisco = await iobras.UpdateZonasRiscoObra(idObra, idMapa);
        if(!canUpdateZonasRisco){
            // Estado da obra não permite atualizar as zonas de risco
            // O idMapa não está na lista de mapas da obra {idObra}
            throw new Exception("Não é possível atualizar as zonas de risco deste Mapa.");
        }
        await imapas.UpdateZonasPerigoOfMapa(idMapa, zonas);       
    }

    public async Task AddMapa(string idObra, IFormFile mapaFile){

        var canChangeMapa = await iobras.CheckIfMapaCanBeChanged(idObra);
        if(!canChangeMapa){
            throw new Exception("Estado atual da Obra não permite alterar o Mapa");
        }

        var listaSvgDBIds = new List<string>();
        var listaSvg = await requestHTTP(mapaFile);                   

        var number = 0;        
        foreach(var svg in listaSvg){
            // new mapa value added to the db
            var ids = await imapas.Add(svg.Key, svg.Value, number);
            if(ids != null)
                listaSvgDBIds.Add(ids);
            number++;
        }
        
        var listaMapasAnteriores = await iobras.AddListaMapaToObra(idObra, listaSvgDBIds);
        await imapas.RemoveMapas(listaMapasAnteriores);
    }

    public async Task UpdateMapaFloorNumber(string idObra, Dictionary<string, int> newFloors){
        var obra = await iobras.GetConstructionById(idObra) ?? throw new Exception("Obra " +idObra+ " não encontrada.");
        var idMapas = obra.Mapa;

        if(idMapas.Except(newFloors.Keys).ToList().Count != 0){
            throw new Exception("Todos os ids dos mapas de uma obra devem estar presentes no valor enviado no HTTP Request");
        }
        
        if(newFloors.Values.Distinct().Count() != newFloors.Values.Count){
            throw new Exception("Não podem haver números de pisos repetidos");
        }

        var orderFloors = newFloors.Values.Order().ToList();
        for (int i = 0; i < orderFloors.Count; i++){
            if(orderFloors[i] != i){
                throw new Exception("Os números dos pisos devem ser >= 0 e sem valores intermédios em 'falta'.");   
            }
        }

        foreach(var pair in newFloors){
            await imapas.UpdateFloorNumber(pair.Key, pair.Value);
        }         
    }

    public async Task<List<Log>> GetLogs(string idObra){
        return await ilogs.GetLogsOfObra(idObra);
    }

    public async Task<List<Log>> GetLogsByDate(string idObra, DateTime date){
        return await ilogs.GetLogsOfObraByDate(idObra, date);
    }

    public async Task<List<Log>> GetDailyLogsCapacete(int nCapacete){
        var idObra = await icapacetes.GetObraIdOfCapacete(nCapacete) ?? throw new Exception("Capacete não está associado a nenhuma obra.");
        return await ilogs.GetDailyLogsCapacete(idObra, nCapacete);
    }

    public async Task MarkLogAsSeen(string id){
        await ilogs.MarkLogAsSeen(id);
    }

    public async Task AddLogs(Log log){
        await ilogs.Add(log);
    }
}
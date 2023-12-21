using iHat.Model.Obras;
using iHat.Model.Capacetes;
using iHat.Model.Logs;
using System.IO.Compression;
using iHat.Model.Mapas;
using iHat.Model.Zonas;

namespace iHat.Model.iHatFacade;

public class iHatFacade: IiHatFacade{

    private readonly IObrasService iobras;
    private readonly ICapacetesService icapacetes;
    private readonly ILogsService ilogs;
    private readonly IMapaService imapas;

    public iHatFacade(IObrasService obrasService, ICapacetesService capacetesService, ILogsService logsService, IMapaService mapasService){
        iobras = obrasService;
        icapacetes = capacetesService;
        ilogs = logsService;
        imapas = mapasService;
    }

    public async Task NewConstruction(string name, IFormFile? mapa, int idResponsavel){

        var listaSvgDBIds = new List<string>();
        if(mapa != null && mapa.Length != 0){

            // Request to python service "model2SVG"
            var listaSvg = new Dictionary<string, string>();

            // POST request to http://127.0.0.1:5000/ifc2sv {"ifc_file": "FILE"}
            using (HttpClient client = new HttpClient())
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    mapa.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
                ByteArrayContent fileContent = new ByteArrayContent(fileBytes);
                content.Add(fileContent, "ifc_file", "ifc_file"); // "file" is the name of the parameter expected by the server

                HttpResponseMessage response = await client.PostAsync("http://127.0.0.1:5000/ifc2svg", content);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return;
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


            foreach(var svg in listaSvg){
                // new mapa value added to the db
                var ids = await imapas.Add(svg.Key, svg.Value);
                if(ids != null)
                    listaSvgDBIds.Add(ids);
            }

        }
        await iobras.AddObra(name, idResponsavel, listaSvgDBIds); 
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

    public async Task<List<Log>> GetLogs(string idObra){
        return await ilogs.GetLogsOfObra(idObra);
    }

    public async Task AddLogs(Log logs){
        await ilogs.Add(logs);
    }


    public async Task ChangeStatusCapacete(int nCapacete, string newStatus){
        await icapacetes.UpdateCapaceteStatus(nCapacete, newStatus);
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

     public async Task<List<Capacete>> GetFreeHelmets(){
        return await icapacetes.GetFreeHelmets();
    }

    public async Task UpdateZonasRiscoObra(string idObra, string idMapa, List<ZonasRisco> zonas){
        await iobras.UpdateZonasRiscoObra(idObra, idMapa, zonas);
    }



    // VERIFICAS OBRAS




}
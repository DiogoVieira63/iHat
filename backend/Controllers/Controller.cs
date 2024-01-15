using FormEncode.Models;
using iHat.Model.Obras;
using iHat.Model.Capacetes;
using iHat.Model.Logs;
using iHat.Model.Zonas;
using Microsoft.AspNetCore.Mvc;
using iHat.Model.MensagensCapacete;
using iHat.MensagensCapacete.Values;
using Microsoft.AspNetCore.Http.HttpResults;

namespace iHat.Controllers;

[ApiController]
[Route("[controller]")] 
public class IHatController : ControllerBase{

    private readonly ILogger<IHatController> _logger;
    private readonly IiHatFacade _facade;

    public IHatController(IiHatFacade facade, ILogger<IHatController> logger){
        _facade = facade;
        _logger = logger;
    }

    /// <summary>
    /// Regista um novo Responsável de Obra
    /// </summary>
    [HttpPost("register")]
    public void RegisterUser(){

    }

    [HttpPost("login")]
    public void LoginUser(){

    }

    /// <summary>
    /// Endpoint que permite alterar o Status de uma Obra "id" para o estado "status"
    /// </summary>
    /// <param name="id">Id da obra a atualizar</param>
    /// <param name="state">Novo estado da obra</param>
    /// <response code="200">Alterou o estado da obra</response>
    /// <response code="400">O pedido recebido não possui um estado válido, 
    /// falhou ao alterar o estado: estado atual da obra é "Finalizada" ou "Cancelada",
    /// ou não a obra "id" não foi encontrada </response>
    [HttpPatch("constructions/{id}/state")]
    public async Task<IActionResult> AlteraEstadoObra(string id, [FromBody] string state) {
        if (string.IsNullOrEmpty(state)) {
            _logger.LogWarning("Cannot change to empty state");
            return BadRequest("Cannot change to empty state");
        }

        try{
            await _facade.UpdateEstadoObra(id, state);
        }
        catch (Exception e){
            return BadRequest(e.Message);
        }
        return Ok();
    }

    /// <summary>
    /// Endpoint que permite atualizar o nome da obra "id" para "name"
    /// </summary>
    /// <param name="id">Id da obra a atualizar</param>
    /// <param name="name">Novo nome da obra</param>
    /// <response code="200">O nome da obra foi atualizado</response>
    /// <response code="400">O pedido não foi executado, 
    /// porque o novo nome não é válido, ou a Obra "id" não foi encontrada,
    /// ou o estado da Obra é "Finalizada" ou "Cancelada"</response>
    [HttpPatch("constructions/{id}")]
    public async Task<IActionResult> UpdateNomeObra(string id, [FromBody] string name ) {
        if (string.IsNullOrEmpty(name)) {
            _logger.LogWarning("New name cannot be empty");
            return BadRequest("New name cannot be empty");
        }

        try{
            await _facade.UpdateNomeObra(id, name);
        }
        catch (Exception e){
            _logger.LogWarning(e.Message);
            return BadRequest(e.Message);
        }

        return Ok();
    }

    
    /// <summary>
    /// Endpoint que permite obter as últimas localizações de todos os capacetes da Obra "id".
    /// </summary>
    /// <param name="id">Id da obra</param>
    /// <returns>Dicionário (numero Capacete, ultima localização do Capacete)</returns>
    [HttpGet("constructions/{id}/helmets/location")]
    public async Task<Dictionary<int, Location>> GetLastLocationCapacetesObra(string id){
        var lista = await _facade.GetLastLocationCapacetesObra(id);
        return lista;
    }

    /// <summary>
    /// Endpoint que permite obter todos os capacetes da obra "id"
    /// </summary>
    /// <param name="id">Id da Obra</param>
    /// <response code="200">A lista dos Capacetes da Obra</response>
    /// <response code="400">O pedido falhou, porque a obra não foi encontrada</response>
    [HttpGet("constructions/{id}/helmets")]
    public async Task<ActionResult<List<Capacete>>> GetAllHelmetsFromObra(string id){
        try{
            var lista = await _facade.GetAllCapacetesdaObra(id);
            return Ok(lista);
        }
        catch(Exception e){
            _logger.LogWarning(e.Message);
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Endpoint que permite obter a obra "id"
    /// </summary>
    /// <param name="id">Id da Obra a obter</param>
    /// <returns>ObraDTO com a informação da Obra "id"</returns>
    /// <response code="200">Obtem a informação da Obra</response>
    /// <response code="400"> Não encontrou a obra, 
    /// porque "id" não é um parametro válido,
    /// ou porque não encontrou a obra </response>
    
    [HttpGet("constructions/{id}")]
    public async Task<ActionResult<ObrasDTO>> GetConstruction(string id){
        if(id == null){
            _logger.LogWarning("Parameter 'id' needs to be defined.");
            return BadRequest("Parameter 'id' needs to be defined.");
        }
            
        var obras = await _facade.GetConstructionById(id);
        if(obras == null){
            _logger.LogWarning("Obra {0} not found.", id);
            return NotFound();
        }

        var mapas = await _facade.GetMapasFromList(obras.Mapa);
        var dto = new ObrasDTO(obras, mapas);
        return dto;
    }

    /// <summary>
    /// Endpoint que retorna uma lista de todas as obras do sistema
    /// </summary>
    /// <returns>Uma lista com as obras</returns>
    [HttpGet("constructions")]
    public async Task<ActionResult<List<Obra>?>> GetConstructions(){
        var lista = await _facade.GetObras(1);
        return lista == null ? NotFound() : lista;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idObra"></param>
    /// <param name="idCapacete"></param>
    /// <returns></returns>
    [HttpPost("constructions/{idObra}/helmets/{idCapacete}")]
    public async Task<IActionResult> AddHelmetToObra(string idObra, string idCapacete){
        try
        {
            int nCapacete = Int32.Parse(idCapacete);
            await _facade.AddCapaceteToObra(nCapacete, idObra);
            return Ok(); 
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("constructions/{id}/map")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> AddMapaToObra(string id, [FromForm] IFormFile Mapa){
        try{
            await _facade.AddMapa(id, Mapa);
        }
        catch(Exception e){
            return BadRequest(e.Message);
        }
        return Ok();
    }

    /*[HttpPost("constructions")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> NewConstruction([FromForm] NewConstructionForm form){
        if(form != null){
            try{
                var idResponsavel = 1;
                if(form.Name != null && form.Mapa != null)
                    await _facade.NewConstruction(form.Name, form.Mapa, idResponsavel);
                else if (form.Name != null)
                    await _facade.NewConstruction(form.Name, null, idResponsavel);
                else
                    throw new Exception("Nome da nova Obra tem de ser indicado");
            }
            catch (Exception e){
                return BadRequest(e.Message);
            }
            return Ok();
        }
        else
            return BadRequest();
    }*/

    [HttpPost("constructions")]
    [DisableRequestSizeLimit]
    public async Task<ActionResult<string?>> NewConstruction([FromForm] NewConstructionForm form){
        if(form != null){
            string? id = null;
            try{
                var idResponsavel = 1;
                if(form.Name != null)
                    id = await _facade.NewConstruction(form.Name, null, idResponsavel);
                else
                    throw new Exception("Nome da nova Obra tem de ser indicado");
            }
            catch (Exception e){
                return BadRequest(e.Message);
            }
            return Ok(id);
        }
        else
            return BadRequest();
    }

    [HttpDelete("constructions/{idObra}/helmets/{idCapacete}")]
    public async Task<IActionResult> DeleteHelmet(string idObra, string idCapacete){
        Console.WriteLine($"Delete Helmet DELETE Request | Obra: {idObra} | Capacete: {idCapacete}");

        // string idObra = "6543c109e272c87c6b5f3d33";

        try
        {
            int nCapacete = Int32.Parse(idCapacete);
            await _facade.RemoveCapaceteFromObra(nCapacete, idObra);
            return Ok(); // Retorna uma resposta de sucesso            
        }
        catch (Exception e)
        {
            return BadRequest(e.Message); // Retorna uma resposta de erro com a mensagem da exceção
        }
    }

    [HttpDelete("constructions/{id}")]
    public async Task<IActionResult> RemoveObraById(string id){
        await _facade.RemoveObraById(id);

        return NoContent(); // Returns 204 No Content -> sucesso
    }

    [HttpPost("helmets")]
    public async Task<IActionResult> NewHelmet(NewHelmetForm form){
        Console.WriteLine("New Helmet POST Request");       
        try
        {   
            await _facade.AddCapacete(form.NCapacete);
            return Ok(); // Retorna uma resposta de sucesso
        }
        catch (Exception e)
        {
            return BadRequest(e.Message); // Retorna uma resposta de erro com a mensagem da exceção
        }
    }
    
    // Permite retornar os últimos 20 dados recebidos do capacete
    [HttpGet("helmets/{id}/data")]
    public async Task<ActionResult<List<MensagemCapacete>?>> GetHelmetData(string id){
        int nCapacete = int.Parse(id);
        var capacetedata = await _facade.GetUltimosDadosDoCapacete(nCapacete);
        if(capacetedata == null){
            return NotFound();
        }
        return capacetedata;
    }

    [HttpGet("helmets/{id}")]
    public async Task<ActionResult<Capacete>> GetHelmet(string id){
        int nCapacete = Int32.Parse(id);
        Console.WriteLine("Get Helmet GET Request {0}", nCapacete);

        var capacete = await _facade.GetCapacete(nCapacete);

        if(capacete == null){
            return NotFound();
        }

        return capacete;
    }

    [HttpGet("helmets")]
    public async Task<ActionResult<List<Capacete>>> GetAllHelmets(){
        var lista = await _facade.GetAllCapacetes();

        if(lista == null){
            return NotFound();
        }

        return lista;
    }

//funciona
    [HttpDelete("helmet/{idCapacete}/{idObra}")]
    public async Task<IActionResult> DeleteHelmet(int idCapacete, string idObra){
        Console.WriteLine("Delete Helmet DELETE Request");

        // string idObra = "6543c109e272c87c6b5f3d33";

        try
        {
            await _facade.RemoveCapaceteFromObra(idCapacete, idObra);
            return Ok(); // Retorna uma resposta de sucesso            
        }
        catch (Exception e)
        {
            return BadRequest(e.Message); // Retorna uma resposta de erro com a mensagem da exceção
        }
    }

// funciona
    [HttpPost("helmet/obra/{idObra}/{idCapacete}")]
    public async Task<IActionResult> AddHelmetToObra(int idCapacete, string idObra){
        Console.WriteLine("Add Helmet To Obra POST Request");

        // string idCapacete = "6543c272e272c87c6b5f3d34";
        // idObra= 6543cd51e272c87c6b5f3d35

        try
        {
            await _facade.AddCapaceteToObra(idCapacete, idObra);
            return Ok(); // Retorna uma resposta de sucesso
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro: {e.Message}");
            return BadRequest(e.Message); // Retorna uma resposta de erro com a mensagem da exceção
        }
    }

    //change estado capacete
    [HttpPatch("helmets/{idCapacete}/newStatus")]
    public async Task<IActionResult> ChangeStatusCapacete(string idCapacete, [FromBody] string newStatus){
        Console.WriteLine("Change Status Capacete PATCH Request");

        try
        {
            int nCapacete = int.Parse(idCapacete);
            await _facade.UpdateCapaceteStatusFromToNaoOperacional(nCapacete, newStatus);
            return Ok(); // Retorna uma resposta de sucesso
        }
        catch (Exception e)
        {
            return BadRequest(e.Message); // Retorna uma resposta de erro com a mensagem da exceção
        }
    }


    [HttpGet("logs/daily/{idObra}")]
    public async Task<ActionResult<List<Log>>> GetDailyLogs(string idObra){

        var lista = await _facade.GetLogsByDate(idObra, DateTime.Today);

        if(lista == null){
            return NotFound();
        }

        return lista;
    }


    [HttpGet("logs/{idObra}/{nCapacete}")]
    public async Task<ActionResult<List<Log>>> GetDailyLogsCapacete(string idObra, string nCapacete){

        int nCap = int.Parse(nCapacete);
        var lista = await _facade.GetDailyLogsCapacete(idObra, nCap);
        return lista;
    }


    [HttpGet("logs/{idObra}")]
    public async Task<ActionResult<List<Log>>> GetLogs(string idObra){

        var lista = await _facade.GetLogs(idObra);
        return lista;
    }

    

    [HttpPost("logs")]
    public async Task<IActionResult> AddLogs(Log logs){
        Console.WriteLine("Add Logs POST Request");

        try
        {
            await _facade.AddLogs(logs);
            return Ok(); // Retorna uma resposta de sucesso
        }
        catch (Exception e)
        {
            return BadRequest(e.Message); // Retorna uma resposta de erro com a mensagem da exceção
        }
    }

    [HttpPatch("logs/{idLog}")]
    public async Task<IActionResult> MarkLogAsSeen(string idLog){
        try{
            await MarkLogAsSeen(idLog);
            return Ok();
        }
        catch(Exception e){
            return BadRequest(e.Message);
        }
    }

    [HttpGet("helmets/free")]
    public async Task<ActionResult<List<Capacete>>> GetFreeHelmets(){
        Console.WriteLine("Get Free Helmets GET Request");

        var lista = await _facade.GetFreeHelmets();

        if(lista == null){
            return NotFound();
        }

        return lista;
    }

    //fazer o Update Zonas de Risco (idObra, {idMapa: List[Zona]})
    [HttpPatch("constructions/{idObra}/map/{idMapa}/zonas")]
    public async Task<IActionResult> UpdateZonasRiscoObra(string idObra, string idMapa, List<ZonasRisco> zonas){
        try
        {
            await _facade.UpdateZonasRiscoObra(idObra, idMapa, zonas);
            return Ok(); // Retorna uma resposta de sucesso
        }
        catch (Exception e)
        {
            return BadRequest(e.Message); // Retorna uma resposta de erro com a mensagem da exceção
        }
    }

    // The dictionary newFloors must have all the maps'ids in Keys;
    // There shouldn't be any Values of the Dictionary repeated.
    [HttpPatch("map/{idObra}")]
    public async Task<IActionResult> UpdateMapaFloorNumber(string idObra, [FromBody] Dictionary<string, int> newFloors){
        foreach(var ids in newFloors)
            _logger.LogWarning(ids.Key);
        await _facade.UpdateMapaFloorNumber(idObra, newFloors);
        return Ok();
    }
}    
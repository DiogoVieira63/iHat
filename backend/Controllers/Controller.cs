using FormEncode.Models;
using iHat.Model.iHatFacade;
using iHat.Model.Obras;
using iHat.Model.Capacetes;
using iHat.Model.Logs;
using Microsoft.AspNetCore.Mvc;
using iHat.Model.MensagensCapacete;

namespace iHat.Controllers;

[ApiController]
[Route("[controller]")] // mudar este nome....
public class IHatController : ControllerBase{

    private readonly ILogger<IHatController> _logger;
    private readonly IiHatFacade _facade;

    public IHatController(IiHatFacade facade, ILogger<IHatController> logger){
        _facade = facade;
        _logger = logger;
    }

    [HttpPost("register")]
    public void RegisterUser(){

    }

    [HttpPost("login")]
    public void LoginUser(){

    }

    [HttpPatch("constructions/{id}/state")]
    public async Task<IActionResult> AlteraEstadoObra(string id, [FromBody] string state) {
        if (string.IsNullOrEmpty(state)) {
            return BadRequest("Cannot change to empty state");
        }

        try{
            await _facade.AlteraEstadoObra(id, state);
        }
        catch (Exception e){
            return BadRequest(e.Message);
        }

        return Ok();
    }

    [HttpPatch("constructions/{id}")]
    public async Task<IActionResult> UpdateNomeObra(string id, [FromBody] string name ) {
        if (string.IsNullOrEmpty(name)) {
            return BadRequest("New name cannot be empty");
        }

        try{
            await _facade.UpdateNomeObra(id, name);
        }
        catch (Exception e){
            return BadRequest(e.Message);
        }

        return Ok();
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

    [HttpGet("constructions/{id}/helmets")]
    public async Task<ActionResult<List<Capacete>>> GetAllHelmetsFromObra(string id){
        var lista = await _facade.GetAllCapacetesdaObra(id);

        if(lista == null){
            return NotFound();
        }

        return lista;
    }

    [HttpGet("constructions/{id}")]
    public async Task<ActionResult<ObrasDTO>> GetConstruction(string id){
        if (id != null){
            var obras = await _facade.GetConstructionById(id);
            var mapas = await _facade.GetMapasDaObra(obras.Mapa);
            var dto = new ObrasDTO(obras, mapas);
            return dto;
        }
        else{
            return NotFound();
        }
    }

    [HttpGet("constructions")]
    public async Task<ActionResult<List<Obra>?>> GetConstructions(){
        var lista = await _facade.GetObras(1);
        return lista == null ? NotFound() : lista;
    }

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

    [HttpPost("constructions/{name}")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> NewConstruction(string name){
        try{
            var idResponsavel = 1;
            if(name != null)
                await _facade.NewConstruction(name, null, idResponsavel);
            else
                throw new Exception("Nome da nova Obra tem de ser indicado");
        }
        catch (Exception e){
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
    }
    */

    [HttpDelete("constructions/{idObra}/helmets/{idCapacete}")]
    public async Task<IActionResult> DeleteHelmet(string idObra, string idCapacete){
        Console.WriteLine($"Delete Helmet DELETE Request | Obra: {idObra} | Capacete: {idCapacete}");

        // string idObra = "6543c109e272c87c6b5f3d33";

        try
        {
            int nCapacete = Int32.Parse(idCapacete);
            await _facade.DeleteCapaceteToObra(nCapacete, idObra);
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
    [HttpGet("helmets/data/{id}")]
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
        Console.WriteLine("Get All Helmets GET Request");

        var lista = await _facade.GetAllCapacetes();

        if(lista == null){
            return NotFound();
        }

        return lista;
    }

    [HttpGet("logs/{idObra}")]
    public async Task<ActionResult<List<Log>>> GetLogs(string idObra){
        Console.WriteLine("Get Logs GET Request");

        var lista = await _facade.GetLogs(idObra);

        if(lista == null){
            return NotFound();
        }

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

}
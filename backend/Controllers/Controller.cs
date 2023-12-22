using FormEncode.Models;
using iHat.Model.iHatFacade;
using iHat.Model.Obras;
using iHat.Model.Capacetes;
using iHat.Model.Logs;
using Microsoft.AspNetCore.Mvc;

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
    

    [HttpGet("constructions")]
    public async Task<ActionResult<List<Obra>?>> GetConstructions(){
        
        var lista = await _facade.GetObras(1);

        if(lista == null){
            return NotFound();
        }
        
        return lista;
    }


    // Get the construction identified by the id
    // ihat/construction/{id}
    [HttpGet("construction/{id}")]
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


    // Input: name, mapa, estado

    [HttpPost("construction")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> NewConstruction([FromForm]NewConstructionForm form){
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

    [HttpPost("atualizarEstado")]
    public void AlteraEstadoObra(string obraId, string novoEstado) {
        _facade.AlteraEstadoObra(obraId, "Tó");
    }

//rever
    [HttpPatch("constructions/{obraId}/{novoNome}")]
    public async Task<IActionResult> UpdateNomeObra(string obraId, string novoNome) {
        Console.WriteLine("New NameObra PATCH Request");
        await _facade.UpdateNomeObra(obraId, novoNome);
        return Ok(); // Return a success response
    }



    [HttpDelete("construction/{id}")]
    public async Task<IActionResult> RemoveObraById(string id){
        await _facade.RemoveObraById(id);

        return NoContent(); // Returns 204 No Content -> sucesso
    }


    [HttpPost("construction/{id}/map")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> AddMapaToObra(string id, [FromForm] IFormFile Mapa){
        try{
            _logger.LogWarning(Mapa.Name);
            await _facade.AddMapa(id, Mapa);
        }
        catch(Exception e){
            return BadRequest(e.Message);
        }
        return Ok();
    }



    [HttpPost("helmet")]
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
    

    [HttpGet("helmet")]
    public async Task<ActionResult<List<Capacete>>> GetAllHelmets(){
        var lista = await _facade.GetAllCapacetes();
        return lista;
    }


    [HttpGet("helmet/{id}")]
    public async Task<ActionResult<Capacete>> GetHelmet(string id){
        int nCapacete = Int32.Parse(id);
        Console.WriteLine("Get Helmet GET Request {0}", nCapacete);

        var capacete = await _facade.GetCapacete(nCapacete);

        if(capacete == null){
            return NotFound();
        }

        return capacete;
    }

// verificar
    [HttpGet("helmet/obra/{idObra}")]
    public async Task<ActionResult<List<Capacete>>> GetAllHelmetsFromObra(string idObra){
        Console.WriteLine("Get All Helmets From Obra GET Request");

        var lista = await _facade.GetAllCapacetesdaObra(idObra);

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
            await _facade.DeleteCapaceteToObra(idCapacete, idObra);
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
        // idObra = 6543cd51e272c87c6b5f3d35

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
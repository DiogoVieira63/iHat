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
    

// verificar
    [HttpGet("constructions/{idObra}/helmets")]
    public async Task<ActionResult<List<Capacete>>> GetAllHelmetsFromObra(string idObra){
        Console.WriteLine("Get All Helmets From Obra GET Request");

        var lista = await _facade.GetAllCapacetesdaObra(idObra);
        // var lista = null;

        if(lista == null){
            return NotFound();
        }

        return lista;
    }

    // Get the construction identified by the id
    // ihat/constructions/{id}
    [HttpGet("constructions/{id}")]
    public async Task<ActionResult<Obra>> GetConstruction(string id){
        if (id != null){
            return await _facade.GetConstructionById(id);
            // return Ok(id);
        }
        else{
            return NotFound();
        }
    }

    [HttpGet("constructions")]
    public async Task<ActionResult<List<Obra>?>> GetConstructions(){
        
        var lista = await _facade.GetObras(1);

        if(lista == null){
            return NotFound();
        }
        
        return lista;
    }

    // Input: name, mapa, estado
    [HttpPost("constructions")]
    public async Task<IActionResult> NewConstruction(NewConstructionForm form){

        _logger.LogWarning(form.Mapa);


        if(form != null){
            try{
                await _facade.NewConstruction(form.Name, form.Mapa, form.Status);
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




    //funcionar
    [HttpPost("helmets")]
    //    public async Task<IActionResult> NewHelmet(Capacete capacete){
    public async Task<IActionResult> NewHelmet(NewHelmetForm form){
        Console.WriteLine("New Helmet POST Request");
        // var capacete = new Capacete("Em uso", "Sem Informação", "", "");
        
        try
        {   
            await _facade.AddHelmet(form.NCapacete);
            return Ok(); // Retorna uma resposta de sucesso
        }
        catch (Exception e)
        {
            return BadRequest(e.Message); // Retorna uma resposta de erro com a mensagem da exceção
        }
    }
    
    //funcionar
    [HttpGet("helmets/{id}")]
    public async Task<ActionResult<Capacete>> GetHelmet(string id){
        Console.WriteLine("Get Helmet GET Request");

        var capacete = await _facade.GetCapacete(id);

        if(capacete == null){
            return NotFound();
        }

        return capacete;
    }

//funcionar
    [HttpGet("helmets")]
    public async Task<ActionResult<List<Capacete>>> GetAllHelmets(){
        Console.WriteLine("Get All Helmets GET Request");

        var lista = await _facade.GetAll();

        if(lista == null){
            return NotFound();
        }

        return lista;
    }


    [HttpDelete("constructions/{id}")]
    public async Task<IActionResult> RemoveObraById(string id){
        await _facade.RemoveObraById(id);

        return NoContent(); // Returns 204 No Content -> sucesso
    }

//funciona
    [HttpDelete("constructions/{idObra}/helmets/{idCapacete}")]
    public async Task<IActionResult> DeleteHelmet(string idCapacete, string idObra){
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
    [HttpPost("constructions/{idObra}/helmets/{idCapacete}")]
    public async Task<IActionResult> AddHelmetToObra(string idCapacete, string idObra){
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
using FormEncode.Models;
using iHat.Model.iHatFacade;
using iHat.Model.Obras;
using iHat.Model.Capacetes;
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
    public async Task<ActionResult<Obra>> GetConstruction(string id){
        if (id != null){
            return await _facade.GetConstructionById(id);
            // return Ok(id);
        }
        else{
            return NotFound();
        }
    }


    // Input: name, mapa, estado
    [HttpPost("construction")]
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




    //funcionar
    [HttpPost("helmet")]
    //    public async Task<IActionResult> NewHelmet(Capacete capacete){
    public async Task<IActionResult> NewHelmet(){
        Console.WriteLine("New Helmet POST Request");
        var capacete = new Capacete("Em uso", "Sem Informação", "", "");
        
        try
        {   
            await _facade.AddHelmet(capacete);
            return Ok(); // Retorna uma resposta de sucesso
        }
        catch (Exception e)
        {
            return BadRequest(e.Message); // Retorna uma resposta de erro com a mensagem da exceção
        }
    }
    
//funcionar
    [HttpGet("helmet")]
    public async Task<ActionResult<List<Capacete>>> GetAllHelmets(){
        Console.WriteLine("Get All Helmets GET Request");

        var lista = await _facade.GetAll();

        if(lista == null){
            return NotFound();
        }

        return lista;
    }

//funcionar
    [HttpGet("helmet/{id}")]
    public async Task<ActionResult<Capacete>> GetHelmet(string id){
        Console.WriteLine("Get Helmet GET Request");

        var capacete = await _facade.GetCapacete(id);

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
        // var lista = null;

        if(lista == null){
            return NotFound();
        }

        return lista;
    }

//funciona
    [HttpDelete("helmet/{idCapacete}/{idObra}")]
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
    [HttpPost("helmet/obra/{idObra}/{idCapacete}")]
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
            return BadRequest(e.Message); // Retorna uma resposta de erro com a mensagem da exceção
        }
    }



   
}
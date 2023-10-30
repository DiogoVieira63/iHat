using iHat.Model.iHatFacade;
using iHat.Model.Obras;
using iHat.Model.Capacetes;
using Microsoft.AspNetCore.Mvc;

namespace iHat.Controllers;

[ApiController]
[Route("[controller]")] // mudar este nome....
public class IHatController : ControllerBase{

    // private readonly ILogger<IHatController> _logger;
    private readonly IiHatFacade _facade;

    public IHatController(IiHatFacade facade){
        _facade = facade;
    }

    [HttpPost("register")]
    public void RegisterUser(){

    }

    [HttpPost("login")]
    public void LoginUser(){

    }

    // [HttpPost("construction")]
    // public void NewConstruction(string name){
    //     Console.WriteLine("New Construction POST Request");

    //     _facade.NewConstruction(name);
    // }

    

    [HttpGet("construction")]
    public async Task<ActionResult<List<Obra>>> GetConstruction(){
        
        var lista = await _facade.GetObras(1);

        if(lista == null){
            return NotFound();
        }
        
        return lista;
    }

    /*[HttpGet("construction\{id}")]
    public void GetConstruction(string id){
    
    }*/

    [HttpPost("helmet")]
   public async Task<IActionResult> NewHelmet(Capacete capacete){
        Console.WriteLine("New Helmet POST Request");
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
    

    [HttpGet("helmet")]
    public async Task<ActionResult<List<Capacete>>> GetAllHelmets(){
        Console.WriteLine("Get All Helmets GET Request");

        var lista = await _facade.GetAll();

        if(lista == null){
            return NotFound();
        }

        return lista;
    }

    [HttpGet("helmet/{id}")]
    public async Task<ActionResult<Capacete>> GetHelmet(string id){
        Console.WriteLine("Get Helmet GET Request");

        var capacete = await _facade.GetCapacete(id);

        if(capacete == null){
            return NotFound();
        }

        return capacete;
    }

    [HttpGet("helmet/obra/{idObra}")]
    public async Task<ActionResult<List<Capacete>>> GetAllHelmetsFromObra(string idObra){
        Console.WriteLine("Get All Helmets From Obra GET Request");

        var lista = await _facade.GetAllCapacetesdaObra(idObra);

        if(lista == null){
            return NotFound();
        }

        return lista;
    }

    [HttpDelete("helmet/{id}")]
    public async Task<IActionResult> DeleteHelmet(string id){
        Console.WriteLine("Delete Helmet DELETE Request");

        try
        {
            await _facade.DeleteCapaceteToObra(id);
            return Ok(); // Retorna uma resposta de sucesso
        }
        catch (Exception e)
        {
            return BadRequest(e.Message); // Retorna uma resposta de erro com a mensagem da exceção
        }
    }

    [HttpPost("helmet/obra/{idObra}")]
    public async Task<IActionResult> AddHelmetToObra(string idCapacete, string idObra){
        Console.WriteLine("Add Helmet To Obra POST Request");

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
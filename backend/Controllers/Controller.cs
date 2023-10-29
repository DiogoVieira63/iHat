using iHat.Model.iHatFacade;
using iHat.Model.Obras;
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

    [HttpPost("helmet")]
    public void NewHelmet(){

    }

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


    [HttpPost("construction")]
    public async Task<IActionResult> NewConstruction(string name){

        try{
            await _facade.NewConstruction(name);
        }
        catch (Exception e){
            return BadRequest(e.Message);
        }
        return Ok();
    }
}
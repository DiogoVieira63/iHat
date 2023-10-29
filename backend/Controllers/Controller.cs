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

    [HttpPost("construction")]
    public void NewConstruction(string name){
        Console.WriteLine("New Construction POST Request");

        _facade.NewConstruction(name);
    }

    [HttpPost("helmet")]
    public void NewHelmet(){

    }

    // Get all constructions managed by the user that made the request
    [HttpGet("constructions")]
    public async Task<ActionResult<List<Obra>>> GetConstruction(){
        
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
            return await _facade.GetObra(id);
            // return Ok(id);
        }
        else{
            return NotFound();
        }
    }
}
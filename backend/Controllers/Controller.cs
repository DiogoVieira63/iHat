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

    [HttpGet("construction")]
    public async Task<ActionResult<List<Obra>>> GetConstruction(){
        
        var lista = await _facade.GetObras(1);

        if(lista == null){
            return NotFound();
        }
        
        return lista;
    }

    [HttpDelete("construction/{id}")]
    public async Task<IActionResult> RemoveObraById(string id){
        await _facade.RemoveObraById(id);

        return NoContent(); // Returns 204 No Content -> sucesso
    }

    /*[HttpGet("construction\{id}")]
    public void GetConstruction(string id){
    
    }*/
}
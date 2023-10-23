using Microsoft.AspNetCore.Mvc;

namespace iHat.Controllers;

[ApiController]
[Route("[controller]")] // mudar este nome....
public class IHatController : ControllerBase{

    // private readonly ILogger<iHatController> _logger;
    private IiHatFacade facade;


    public IHatController(){

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

        facade.NewConstruction(name);
    
    }

    [HttpPost("helmet")]
    public void NewHelmet(){

    }

    [HttpGet("construction")]
    public void GetConstruction(){

    }

    /*[HttpGet("construction\{id}")]
    public void GetConstruction(string id){
    
    }*/
}
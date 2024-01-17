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
    /// <response code="404">Não encontrou a Obra</response>
    /// <response code="400">"id" não é um parametro válido</response>
    
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
    public async Task<ActionResult<List<Obra>>> GetConstructions(){
        var lista = await _facade.GetObras(1);
        return lista;
    }

    /// <summary>
    /// Função que permite adicionar o capacete {idCapacete} à obra {idObra}
    /// </summary>
    /// <param name="idObra">Id da obra a que se vai adicionar o capacete</param>
    /// <param name="idCapacete">Numero de Capacete</param>
    /// <response code="200">Adicionou o capacete à obra</response>
    /// <response code="400">Falha se não encontrar o capacete {idCapacete},
    /// ou se não encontrar a obra,
    /// ou se o estado atual da obra não permitir adicionar um novo capacete,
    /// ou se o capacete já tiver associado a obra </response>
    [HttpPost("constructions/{idObra}/helmets/{idCapacete}")]
    public async Task<IActionResult> AddHelmetToObra(string idObra, string idCapacete){
        try
        {
            int nCapacete = int.Parse(idCapacete);
            await _facade.AddCapaceteToObra(nCapacete, idObra);
            return Ok(); 
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    /// <summary>
    /// Função que permite atualizar o mapa de uma obra {id}
    /// </summary>
    /// <param name="id">Id da Obra</param>
    /// <param name="Mapa">Ficheiro IFC 3D que contem o novo mapa</param>
    /// <response code="200">Adicionou o mapa à obra</response>
    /// <response code="400">Se o estado da obra não permitir atualizar o mapa, 
    /// ou se a Obra não for encontrada </response>
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


    /// <summary>
    /// Função que permite criar uma nova obra ao sistema
    /// </summary>
    /// <param name="form">Formulário que contem as informações necessárias para criar a nova obra</param>
    /// <returns>Id da nova Obra</returns>
    /// <response code="200">Criou a Obra com a informação indicada</response>
    /// <response code="400">Já existe uma Obra com o nome indicado</response>
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


    /// <summary>
    /// Função que permite remover um Capacete {idCapacete} da Obra {idObra}
    /// </summary>
    /// <param name="idObra">Id da Obra de onde se irá remover o capacete</param>
    /// <param name="idCapacete">Número do Capacete a remover</param>
    /// <response code="200">Removeu o capacete da Obra</response>
    /// <response code="400">Se o capacete não estiver associado à Obra {idObra}, 
    /// ou se um capacete estiver associado a um Trabalhador </response>
    [HttpDelete("constructions/{idObra}/helmets/{idCapacete}")]
    public async Task<IActionResult> DeleteHelmet(string idObra, string idCapacete){
        try
        {
            int nCapacete = int.Parse(idCapacete);
            await _facade.RemoveCapaceteFromObra(nCapacete, idObra);
            return Ok();        
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    /// <summary>
    /// Função que permite remover a Obra {id} do sistema.
    /// </summary>
    /// <param name="id">Id da Obra a remover</param>
    [HttpDelete("constructions/{id}")]
    public async Task<IActionResult> RemoveObraById(string id){
        await _facade.RemoveObraById(id);
        return NoContent();
    }

    /// <summary>
    /// Função que permite adicionar um novo Capacete ao Sistema
    /// </summary>
    /// <param name="form">Formulário que deverá conter o Número do Capacete a adicionar</param>
    /// <response code="200">Adicionou o capacete da Obra</response>
    /// <response code="400">Já existe um capacete com o mesmo número no sistema</response>
    [HttpPost("helmets")]
    public async Task<IActionResult> NewHelmet(NewHelmetForm form){ 
        try
        {   
            await _facade.AddCapacete(form.NCapacete);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    /// <summary>
    /// Função que permite obter os últimos 20 dados recebidos do capacete {id}
    /// </summary>
    /// <param name="id">Numero do capacete</param>
    /// <returns>Lista com os dados recebidos do Capacete</returns>
    [HttpGet("helmets/{id}/data")]
    public async Task<ActionResult<List<MensagemCapacete>?>> GetHelmetData(string id){
        int nCapacete = int.Parse(id);
        var capacetedata = await _facade.GetUltimosDadosDoCapacete(nCapacete);
        return capacetedata;
    }

    /// <summary>
    /// Função que permite obter o Capacete {id}
    /// </summary>
    /// <param name="id">Numero do Capacete a Obter</param>
    /// <returns>Capacete com Numero {id} </returns>
    /// <response code="200">Encontrou o capacete</response>
    /// <response code="404">Não encontrou o capacete</response>
    [HttpGet("helmets/{id}")]
    public async Task<ActionResult<Capacete>> GetHelmet(string id){
        int nCapacete = int.Parse(id);
        var capacete = await _facade.GetCapacete(nCapacete);
        if(capacete == null){
            return NotFound();
        }
        return capacete;
    }

    /// <summary>
    /// Função que permite obter todos os capacetes do sistema
    /// </summary>
    /// <returns>Lista dos capacetes encontrados</returns>
    [HttpGet("helmets")]
    public async Task<ActionResult<List<Capacete>>> GetAllHelmets(){
        var lista = await _facade.GetAllCapacetes();
        return lista;
    }

    /// <summary>
    /// Função que permite alterar o status do capacete {idCapacete}.
    /// Esta função só permite alterar o estado do capacete para não operacional ou de não operacional para livre.
    /// </summary>
    /// <param name="idCapacete">Numero do Capacete a alterar</param>
    /// <param name="newStatus">Novo estado do capacete</param>
    /// <response code="200">Alterou o estado do capacete</response>
    /// <response code="400">Não encontrou o capacete</response>
    [HttpPatch("helmets/{idCapacete}/newStatus")]
    public async Task<IActionResult> ChangeStatusCapacete(string idCapacete, [FromBody] string newStatus){
        try
        {
            int nCapacete = int.Parse(idCapacete);
            await _facade.UpdateCapaceteStatusFromToNaoOperacional(nCapacete, newStatus);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Função que permite obter os Logs da obra {idObra}, criados no dia em que este endpoint foi chamado
    /// </summary>
    /// <param name="idObra">Id da Obra</param>
    /// <returns>Lista de Logs da Obra</returns>
    [HttpGet("logs/daily/{idObra}")]
    public async Task<ActionResult<List<Log>>> GetDailyLogs(string idObra){
        var lista = await _facade.GetLogsByDate(idObra, DateTime.Today);
        return lista;
    }

    /// <summary>
    /// Função que permite obter os logs do Capacete {nCapacete}. 
    /// Logs devolvidos foram criados no dia em este endpoint foi chamado e para a Obra a que o capacete está associado.
    /// </summary>
    /// <param name="nCapacete">Número do Capacete</param>
    /// <returns>Lista dos Logs</returns>
    /// <response code="200">Obteve a lista de logs</response>
    /// <response code="400">Não encontrou o capacete,
    /// ou o capacete não estava associado a nenhuma Obra,
    /// ou o parsing do Numero do capacete de string para int falhou</response>
    [HttpGet("logs/helmet/{nCapacete}")]
    public async Task<ActionResult<List<Log>>> GetDailyLogsCapacete(string nCapacete){

        try{
            int nCap = int.Parse(nCapacete);
            var lista = await _facade.GetDailyLogsCapacete(nCap);
            return lista;
        }
        catch(Exception e){
            return BadRequest(e.Message);
        }
        
    }

    /// <summary>
    /// Função que permite obter todos os Logs de uma Obra {idObra}
    /// </summary>
    /// <param name="idObra">Id da Obra</param>
    /// <returns>Lista de Logs da Obra</returns>
    [HttpGet("logs/{idObra}")]
    public async Task<ActionResult<List<Log>>> GetLogs(string idObra){
        var lista = await _facade.GetLogs(idObra);
        return lista;
    }

    
    /*[HttpPost("logs")]
    public async Task<IActionResult> AddLogs(Log logs){
        try
        {
            await _facade.AddLogs(logs);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }*/

    /// <summary>
    /// Função que permite atualizar o Log {idLog} para Visto
    /// </summary>
    /// <param name="idLog">Id do Log a atualizar</param>
    /// <response code="200">Marcou o Log como visto</response>
    /// <response code="404">Não encontrou o Log</response>
    [HttpPatch("logs/{idLog}")]
    public async Task<IActionResult> MarkLogAsSeen(string idLog){
        try{
            await _facade.MarkLogAsSeen(idLog);
            return Ok();
        }
        catch(Exception e){
            return NotFound(e.Message);
        }
    }

    /// <summary>
    /// Função que permite obter todos os capacetes Livres do sistema, ie, todos os capacetes que não estejam associados a nenhuma obra, nem a nenhum trabalhador e que estejam operacionais.
    /// </summary>
    /// <returns>Lista dos Capacetes Livres</returns>
    [HttpGet("helmets/free")]
    public async Task<ActionResult<List<Capacete>>> GetFreeHelmets(){
        var lista = await _facade.GetFreeHelmets();
        return lista;
    }


    /// <summary>
    /// Função que permite atualizar as zonas de risco do mapa {idMapa} da Obra {idObra}
    /// </summary>
    /// <param name="idObra">Id da Obra que contém o Mapa</param>
    /// <param name="idMapa">Id do Mapa ao qual se vai alterar as zonas de risco</param>
    /// <param name="zonas">Novas Zonas de risco</param>
    /// <response code="200">Conseguiu alterar as zonas de risco</response>
    /// <response code="400">Não encontrou a obra,
    /// ou não encontrou nenhum mapa {idMapa} na obra {idObra},
    /// ou o estado atual da obra não permite atualizar as zonas de risco</response>
    [HttpPatch("constructions/{idObra}/map/{idMapa}/zonas")]
    public async Task<IActionResult> UpdateZonasRiscoObra(string idObra, string idMapa, List<ZonasRisco> zonas){
        try
        {
            await _facade.UpdateZonasRiscoObra(idObra, idMapa, zonas);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // The dictionary newFloors must have all the maps'ids in Keys;
    // There shouldn't be any Values of the Dictionary repeated.

    /// <summary>
    /// Função que permite atualizar o piso de cada SVG que compõe o mapa da Obra
    /// </summary>
    /// <param name="idObra">Id da Obra a alterar</param>
    /// <param name="newFloors">Um dicionário que associa cada id de um mapa da obra a um novo piso</param>
    /// <response code="200">Alterou os pisos da Obra</response>
    /// <response code="400">Não encontrou a obra,
    /// ou se houver pisos repetidos no newFloors,
    /// ou se houver algum piso negativo,
    /// ou se faltar algum id de um mapa no newFloors</response>
    [HttpPatch("map/{idObra}")]
    public async Task<IActionResult> UpdateMapaFloorNumber(string idObra, [FromBody] Dictionary<string, int> newFloors){
        await _facade.UpdateMapaFloorNumber(idObra, newFloors);
        return Ok();
    }
}    
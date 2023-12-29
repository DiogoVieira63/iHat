using iHat.Model.Logs;
using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs;

public sealed class LogsHub : Hub {
    /*
    variável de instância que mantêm um mapeamente entre o id das obras
    e as conexões que deverá notificar quando um novo log for "criado"
    */
    private static Dictionary<string, HashSet<string>> _obrasConnection = new Dictionary<string, HashSet<string>>();

    /*
    Começa uma nova conexão e adiciona essa conexão à lista de conexões a notificar quando 
    os logs de uma obraID forem alterados (adicionados)
    */
    public void RegistarConexao(string obraId){
        var connectionId = Context.ConnectionId;
        if(!_obrasConnection.ContainsKey(obraId)){
            _obrasConnection[obraId] = new HashSet<string>();
        }
        _obrasConnection[obraId].Add(connectionId);
    }

    public async Task RegistarConexaoGroup(string obraId){
        await Groups.AddToGroupAsync(Context.ConnectionId, obraId);
    }

    public override async Task OnConnectedAsync(){
        Console.WriteLine("Connection Request");
        var obra_id = Context.GetHttpContext()!.Request.Query["obra_id"];
        var obraIdString = obra_id.ToString();
        Console.WriteLine("obraIdString: {0}", obraIdString);
        if(obraIdString != null){
            // RegistarConexao(obraIdString);
            await RegistarConexaoGroup(obraIdString);
            await base.OnConnectedAsync();
            Console.WriteLine("Connection Concluded");
        }
        else
            throw new Exception("Subscription for log's updates failed: 'obra_id' is a mandatory parameter.");
    }

    /*
    Função que remove a conexão das conexões a notificar 
    quando os logs da obraId forem alterados
    */
    public void DesregistarConexao(string obraId){
        var connectionId = Context.ConnectionId;
        if(_obrasConnection.ContainsKey(obraId)){
            _obrasConnection[obraId].Remove(connectionId);
            if(_obrasConnection[obraId].Count == 0){
                _obrasConnection.Remove(obraId);
            }
        }
    }

    public async Task DesregistarConexaoGroup(string obraId){
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, obraId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception){
        var obra_id = Context.GetHttpContext()!.Request.Query["obra_id"];
        var obraIdString = obra_id.ToString();
        if(obraIdString != null){
            // DesregistarConexao(obraIdString);
            await DesregistarConexaoGroup(obraIdString);
            await base.OnDisconnectedAsync(exception);
        }
        else 
            throw new Exception("Cancelling subscription for log's updates failed: 'obra_id' is a mandatory parameter.");

    }

    /*
    Função que será chamada quando um novo log for adicionado para a obraId
    */
    public async Task NotifyClient(string obraId, List<Log> logs){
        if(_obrasConnection.ContainsKey(obraId)){
            foreach(var connectionId in _obrasConnection[obraId]){
                await Clients.Client(connectionId).SendAsync("UpdateLogs", logs);
            }
        }
    }

    public async Task NotifyClientGroups(string obraId, List<Log> logs){
        await Clients.Group(obraId).SendAsync("UpdateLogs", logs);
    }

}
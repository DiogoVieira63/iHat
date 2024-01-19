using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs;

public sealed class ObrasHub : Hub {
    /*
    Começa uma nova conexão e adiciona essa conexão à lista de conexões a notificar quando 
    os logs de uma obraID forem alterados (adicionados)
    */
   
    public async Task RegistarConexaoGroup(string obraId){
        await Groups.AddToGroupAsync(Context.ConnectionId, obraId);
    }

    public override async Task OnConnectedAsync(){
        var obra_id = Context.GetHttpContext()!.Request.Query["obra_id"];
        var obraIdString = obra_id.ToString();
        Console.WriteLine("obraIdString: {0}", obraIdString);
        if(obraIdString != null){
            await RegistarConexaoGroup(obraIdString);
            await base.OnConnectedAsync();
            Console.WriteLine("Connection Started");
        }
        else
            throw new Exception("Subscription for logs and helmet's location updates failed: 'obra_id' is a mandatory parameter.");
    }

    /*
    Função que remove a conexão das conexões a notificar 
    quando os logs da obraId forem alterados
    */
    
    public async Task DesregistarConexaoGroup(string obraId){
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, obraId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception){
        var obra_id = Context.GetHttpContext()!.Request.Query["obra_id"];
        var obraIdString = obra_id.ToString();
        if(obraIdString != null){
            await DesregistarConexaoGroup(obraIdString);
            await base.OnDisconnectedAsync(exception);
        }
        else 
            throw new Exception("Cancelling subscription for logs and helmet's location updates failed: 'obra_id' is a mandatory parameter.");

    }
}
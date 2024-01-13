using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs;

public sealed class DadosCapaceteHub : Hub {

    public async Task RegistarConexaoGroup(string capaceteId){
        await Groups.AddToGroupAsync(Context.ConnectionId, capaceteId);
    }   

    public override async Task OnConnectedAsync(){
        var capacete_id = Context.GetHttpContext()!.Request.Query["capacete_id"];
        var capaceteIdString = capacete_id.ToString();
        if(capaceteIdString != null){
            await RegistarConexaoGroup(capaceteIdString);
            await base.OnConnectedAsync();
            Console.WriteLine("Connection to {0} Group Started", capaceteIdString);
        }
        else
            throw new Exception("Subscription for log's updates failed: 'capacete_id' is a mandatory parameter.");
    }

    public async Task DesregistarConexaoGroup(string capaceteId){
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, capaceteId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception){
        var capacete_id = Context.GetHttpContext()!.Request.Query["capacete_id"];
        var capaceteIdString = capacete_id.ToString();
        if(capaceteIdString != null){
            await DesregistarConexaoGroup(capaceteIdString);
            await base.OnDisconnectedAsync(exception);
            Console.WriteLine("Connection to {0} Group Ended", capaceteIdString);
        }
        else 
            throw new Exception("Cancelling subscription for log's updates failed: 'capacete_id' is a mandatory parameter.");
    }
}
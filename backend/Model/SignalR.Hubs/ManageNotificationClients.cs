
using iHat.MensagensCapacete.Values;
using iHat.Model.Logs;
using iHat.Model.MensagensCapacete;
using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs;

public sealed class ManageNotificationClients{

    private IHubContext<DadosCapaceteHub> _dadosCapaceteHub;
    private IHubContext<ObrasHub> _obrasHub;

    public ManageNotificationClients(IHubContext<DadosCapaceteHub> dadosCapaceteHub, IHubContext<ObrasHub> obrasHub){
        _dadosCapaceteHub = dadosCapaceteHub;
        _obrasHub = obrasHub; 
    }

    /*
    Função que notifica todos os clientes do grupo {obraId} do hub ObrasHub com a última localização de um capacete da {obraId}
    */
    public async Task NotifyClientsObraWithSingleLocation(string obraId, Dictionary<int, Location> dict){
        await _obrasHub.Clients.Group(obraId).SendAsync("UpdateSingleLocation", dict);
    }

    /*
    Função que notifica todos os clientes do grupo {obraId} do hub ObrasHub com as ultimas localizações de todos os capacetes de uma {obraId}
    */
    public async Task NotifyClientsObraWithMultipleLocations(string obraId, Dictionary<int, Location> allCapacetesLocation){
        await _obrasHub.Clients.Group(obraId).SendAsync("UpdateAllLocation", allCapacetesLocation);
    }

    /*
    Função que notifica todos os clientes do grupo {obraId} do hub ObrasHub com o último log da obra {obraId}
    */
    public async Task NotifyClientsObraWithLastLogs(string obraId, Log allLogs){
        await _obrasHub.Clients.Group(obraId).SendAsync("UpdateLogs", allLogs);
    }

    public async Task NotifyClientsCapaceteWithLastLog(int nCapacete, Log dailyLogs){
        await _dadosCapaceteHub.Clients.Group(nCapacete.ToString()).SendAsync("UpdateLogsCapacete", dailyLogs);
    }

    /*
    Função que notifica todos os clientes do grupo {obraId} do hub ObrasHub com o número do capacete ("nCapacete") da {obraId} que está dentro da zona de risco
    */
    public async Task NotifyClientsObraCapaceteDentroZonaRisco(string obraId, int nCapacete){
        await _obrasHub.Clients.Group(obraId).SendAsync("UpdateCapaceteDentroZonaRisco", nCapacete);
    }

    /*
    Função que notifica todos os clientes do grupo {nCapacete} do hub DadosCapaceteHub com os últimos dados recebidos do capacete {nCapacete} 
    */
    public async Task NotifyClientsCapaceteWithLastMessage(int nCapacete, MensagemCapacete lastReceivedMessage){
        await _dadosCapaceteHub.Clients.Group(nCapacete.ToString()).SendAsync("UpdateDadosCapacete", lastReceivedMessage);
    }




    


}
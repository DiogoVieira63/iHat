
using iHat.MensagensCapacete.Values;
using iHat.Model.Logs;
using iHat.Model.MensagensCapacete;
using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs;

public sealed class ManageNotificationClients{

    private IHubContext<DadosCapaceteHub> _dadosCapaceteHub;
    private IHubContext<ObrasHub> _obrasHub;

    public ManageNotificationClients(IHubContext<DadosCapaceteHub> dadosCapaceteHub, IHubContext<ObrasHub> obrasHub){
        _dadosCapaceteHub = dadosCapaceteHub; // helmetdata
        _obrasHub = obrasHub; // obras
    }

    public async Task NotifyClientsObraWithSingleLocation(string obraId, Dictionary<int, Location> dict){
        await _obrasHub.Clients.Group(obraId).SendAsync("UpdateSingleLocation", dict);
    }

    public async Task NotifyClientsObraWithMultipleLocations(string obraId, List<Location> allCapacetesLocation){
        await _obrasHub.Clients.Group(obraId).SendAsync("UpdateAllLocation", allCapacetesLocation);
    }

    public async Task NotifyClientsObraWithAllLogs(string obraId, List<Log> allLogs){
        await _obrasHub.Clients.Group(obraId).SendAsync("UpdateLogs", allLogs);
    }
    
    public async Task NotifyClientsCapaceteWithLastMessage(int nCapacete, MensagemCapacete lastReceivedMessage){
        await _dadosCapaceteHub.Clients.Group(nCapacete.ToString()).SendAsync("UpdateDadosCapacete", lastReceivedMessage);
    }
}

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

    public async Task NotifyClientsObraWithMultipleLocations(string obraId, Dictionary<int, Location> allCapacetesLocation){
        await _obrasHub.Clients.Group(obraId).SendAsync("UpdateAllLocation", allCapacetesLocation);
    }

    public async Task NotifyClientsObraWithLastLogs(string obraId, Log allLogs){
        await _obrasHub.Clients.Group(obraId).SendAsync("UpdateLogs", allLogs);
    }

    public async Task NotifyClientsObraCapaceteDentroZonaRisco(string obraId, int nCapacete){
        await _obrasHub.Clients.Group(obraId).SendAsync("UpdateCapaceteDentroZonaRisco", nCapacete);
    }
    
    public async Task NotifyClientsCapaceteWithLastMessage(int nCapacete, MensagemCapacete lastReceivedMessage){
        await _dadosCapaceteHub.Clients.Group(nCapacete.ToString()).SendAsync("UpdateDadosCapacete", lastReceivedMessage);
    }

    public async Task NotifyClientsCapaceteWithLastLog(int nCapacete, Log dailyLogs){
        await _dadosCapaceteHub.Clients.Group(nCapacete.ToString()).SendAsync("UpdateLogsCapacete", dailyLogs);
    }

}
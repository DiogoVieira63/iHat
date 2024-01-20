// Import the SignalR library
import type { Log, MensagemCapacete } from '@/interfaces';
import * as signalR from '@microsoft/signalr';

export class CapaceteSignalRService {
    connection: signalR.HubConnection

    constructor(idCapacete: string) {
        this.connection = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.None)
            .withUrl('http://localhost:5069/helmetdata?capacete_id=' + idCapacete, {
                skipNegotiation: true,
                transport: signalR.HttpTransportType.WebSockets
            })
            .build()
    }

    start() {
        return this.connection
            .start()
            .then(() => {
                console.log('SignalR Connected.')
            })
            .catch((err) => {
                console.log(err)
            })
    }

    close() {
        this.connection.stop().then(() => {
            console.log('SignalR Disconnected.')
        })
    }

    updateCapaceteData(callback: (updatedCapaceteData: MensagemCapacete) => void) {
        this.connection.on('UpdateDadosCapacete', (updatedCapaceteData) => {
            console.log('Received message:', updatedCapaceteData)
            callback(updatedCapaceteData)
        })
    }
    updateCapaceteLogs(callback: (updatedCapaceteLogs: Log) => void) {
        this.connection.on("UpdateLogsCapacete", (updatedCapaceteLogs) => {
            console.log("Received new Log:", updatedCapaceteLogs);
            callback(updatedCapaceteLogs);
        });
    }

}

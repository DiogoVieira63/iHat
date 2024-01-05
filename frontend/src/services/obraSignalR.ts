// Import the SignalR library
import * as signalR from '@microsoft/signalr';
import { type Log } from '@/interfaces';

export class ObraSignalRService {
    connection : signalR.HubConnection

    constructor(idObra: string){
        this.connection = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Debug)
            .withUrl("http://localhost:5069/obra?obra_id=" + idObra, {
              skipNegotiation: true,
              transport: signalR.HttpTransportType.WebSockets
            })
            .build();

        // this.connection.on("updatelogs", (message) => {
        //     console.log("Received message:", message);
        // });

        try {
            this.connection.start().then(() => {
            console.log("SignalR Connected.");
            });
        } catch (err) {
            console.log(err);
        }
    }

    handleIncomingLogs(callback: (updatedLogs: Array<Log>) => void) {
        this.connection.on("updatelogs", (updatedLogs) => {
            console.log("Received message:", updatedLogs);
            callback(updatedLogs);
        });
    }
}
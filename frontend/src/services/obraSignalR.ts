// Import the SignalR library
import * as signalR from '@microsoft/signalr';


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

        // this.connection.on("ReceiveMessage", (message) => {
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


    close(){
        this.connection.stop().then(() => {
            console.log("SignalR Disconnected.");
        });
    }

    //Pass the
    updateCapacetePosition(func: Function){
        this.connection.on("UpdateSingleLocation", (message) => {
            const id = Object.keys(message)[0];
            func(Number(id), message[id]);
        });
    }

    updateLogs(func: Function){
        this.connection.on("UpdateLogs", (message) => {
            console.log("Received message:", message);
            func(message);
        });
    }

}
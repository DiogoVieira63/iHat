// Import the SignalR library
import * as signalR from '@microsoft/signalr';
import type { Position, Log} from '@/interfaces';

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
        // try {
        //     this.connection.start().then(() => {
        //     console.log("SignalR Connected.");
        //     });
        // } catch (err) {
        //     console.log(err);
        // }
    }


    start(){
        return this.connection.start().then(() => {
            console.log("SignalR Connected.");
        }
        ).catch((err) => {
            console.log(err);
        });
    }

    close(){
        this.connection.stop().then(() => {
            console.log("SignalR Disconnected.");
        });
    }

    updateCapacetePosition(callback: (capaceteId: number, position: Position) => void) {
        this.connection.on("UpdateSingleLocation", (message) => {
            const id = Object.keys(message)[0];
            callback(Number(id), message[id]);
        });
    }

    // updateLogs(func: Function){
    //     this.connection.on("UpdateLogs", (message) => {
    //         console.log("Received message:", message);
    //         func(message);
    //     });
    // }

    handleIncomingLogs(callback: (updatedLogs: Array<Log>) => void) {
        this.connection.on("updatelogs", (updatedLogs) => {
            console.log("Received message:", updatedLogs);
            callback(updatedLogs);
        });
    }
}
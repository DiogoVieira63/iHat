// Import the SignalR library
import * as signalR from '@microsoft/signalr';


export class SignalRService {
    connection : signalR.HubConnection

    constructor(connectPoint: string){
        this.connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5069/" + connectPoint)
        .build();

        // this.connection.on("ReceiveMessage", (message) => {
        //     console.log("Received message:", message);
        // });
    
        // Start the connection
        // this.connection.start()
        // .then(() => {
        //     console.log("Connection established");
        //     // this.connection.invoke("SendMessage", "Hello");
        // })
        // .catch((err) => {
        //     console.error(err.toString());
        // });

        try {
            this.connection.start().then(() => {
            console.log("SignalR Connected.");
            });
        } catch (err) {
            console.log(err);
        }
    }
}
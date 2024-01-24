// Import the SignalR library
import type { Log, Position } from '@/interfaces'
import * as signalR from '@microsoft/signalr'

export class ObraSignalRService {
    connection: signalR.HubConnection

    constructor(idObra: string) {
        this.connection = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.None)
            .withUrl('http://localhost:5069/obra?obra_id=' + idObra, {
                skipNegotiation: true,
                transport: signalR.HttpTransportType.WebSockets
            })
            .build()
    }

    start() {
        return this.connection
            .start()
            .then(() => {
                console.log('SignalR Connected to ' + this.connection.baseUrl)
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

    updateCapacetePosition(callback: (capaceteId: number, position: Position) => void) {
        this.connection.on('UpdateSingleLocation', (message) => {
            console.log('UpdateSingleLocation',message)
            const id = Object.keys(message)[0]
            callback(Number(id), message[id])
        })
    }

    offCapacetePosition() {
        this.connection.off('UpdateSingleLocation')
    }
    
    handleIncomingLogs(callback: (updatedLog: Log) => void) {
        this.connection.on('UpdateLogs', (updatedLog) => {
            callback(updatedLog)
        })
    }
}

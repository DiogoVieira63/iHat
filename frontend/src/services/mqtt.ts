import type { IClientOptions, MqttClient } from 'mqtt'
import mqtt from 'mqtt'

export class MqttService {
    client: MqttClient | undefined
    connected = false

    connect() {
        return new Promise((resolve, reject) => {
            const options: IClientOptions = {
                clientId: 'admin',
                username: 'admin',
                password: 'password'
            }
            console.log('Connecting to MQTT broker...')

            this.client = mqtt.connect('ws://localhost:9001', options)

            this.client.on('connect', () => {
                console.log('Connected to MQTT broker')
                this.connected = true
                resolve(true)
            })

            this.client.on('error', (err) => {
                console.error(`MQTT error: ${err}`)
                reject(err)
            })
        })
    }


    async publish(topic: string, message: string) {
        if (!this.client) {
            console.error('MQTT client not connected')
            return
        }
        this.client.publish(topic, message, (err) => {
            if (err) {
                console.error(`Failed to publish: ${err}`)
            }
        })
    }

    async subscribe(topic: string, callback: (topic: string, message: string) => void) {
        if (!this.client) {
            console.error('MQTT client not connected')
            return
        }
        this.client.subscribe(topic, (err) => {
            if (err) {
                console.error(`Failed to subscribe: ${err}`)
            } else {
                console.log(`Subscribed to ${topic}`)
            }
        })

        this.client.on('message', (topic, message) => {
            callback(topic, message.toString())
        })
    }
}

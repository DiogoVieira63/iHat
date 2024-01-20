import type { IClientOptions, MqttClient } from 'mqtt'
import mqtt from 'mqtt'

export class MqttService {
    client: MqttClient

    constructor(client: MqttClient | undefined) {
        if (client === undefined) {
            const options: IClientOptions = {
                clientId: 'admin',
                username: 'admin',
                password: 'password'
            }
            console.log('Connecting to MQTT broker...')

            this.client = mqtt.connect('ws://localhost:9001', options)

            this.client.on('connect', () => {
                console.log('Connected to MQTT broker')
                this.client.publish('presence', 'Hello mqtt', (err) => {
                    if (err) {
                        console.error(`Failed to publish: ${err}`)
                    }
                })
            })
        } else {
            this.client = client
        }

        this.client.on('error', (err) => {
            console.error(`MQTT error: ${err}`)
        })
    }

    async publish(topic: string, message: string) {
        this.client.publish(topic, message, (err) => {
            if (err) {
                console.error(`Failed to publish: ${err}`)
            }
        })
    }

    async subscribe(topic: string, callback: (topic: string, message: string) => void) {
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

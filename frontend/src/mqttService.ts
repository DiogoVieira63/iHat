import type { MqttClient, IClientOptions } from "mqtt";
import mqtt from "mqtt";

export class MqttService {
  client: MqttClient;

  constructor(client : MqttClient | undefined) {
    if (client === undefined){
      const options: IClientOptions = {
        clientId: 'myUsername',
      };
      console.log('Connecting to MQTT broker...');
  
      this.client = mqtt.connect('ws://localhost:8883', options);
  
      this.client.on("connect", () => {
          console.log("Connected to MQTT broker");
          this.client.publish("presence", "Hello mqtt", (err) => {
            if (err) {
              console.error(`Failed to publish: ${err}`);
            }
          });
      });
    }
    else {
      this.client = client
    }

    this.client.on("error", (err) => {
      console.error(`MQTT error: ${err}`);
    });
  }

  async publish(topic: string, message: string) {
    this.client.publish(topic, message, (err) => {
      if (err) {
        console.error(`Failed to publish: ${err}`);
      }
    });
  }
}
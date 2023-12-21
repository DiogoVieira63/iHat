// src/store/index.ts
import { defineStore } from 'pinia'
import type { Input } from '@/views/SimulatorView.vue'
import { MqttService } from '@/mqttService'
import { encode } from 'punycode'


interface Task {
    inputs: Array<Input>
    intervalId?: NodeJS.Timeout
    intervalSeconds: number
    capacetes: Array<number>
}

interface DataMQTT {
    HelmetNB: number
    Fall: boolean
    BodyTemperature: number
    Heartrate: number
    Proximity: number
    Location: {
        X: number
        Y: number
        Z: number
    }
    Gases: {
        Metano: number
        MonoxidoCarbono: number
    }
}

/*
        result.Add("HelmetNB", "1");
        result.Add("TypeMessage","ValueUpdate");
        result.Add("Fall", "False");
        result.Add("BodyTemperature", 36.1);
        result.Add("Heartrate", 100);
        result.Add("Proximity", "10");
        result.Add("Position", "?");



        JObject loc = new JObject();
        loc.Add("X", 0);
        loc.Add("Y", 0);
        loc.Add("Z", 0);
        result.Add("Location", loc);

        JObject gases = new JObject();
        gases.Add("Metano", 0);
        gases.Add("MonoxidoCarbono", 0);

        result.Add("Gases", gases);
*/



const random = (min: number, max: number) => {
    return Math.random() * (max - min) + min;
}
const envio = (mqtt : MqttService, task : Task) => {
    const { inputs, capacetes } = task
    for(const idCapacete of capacetes){
        const input = inputs.map((item) => {
            if (item.tipo == 'Constante') {
                return {
                    title: item.title,
                    value: item.value[0]
                }
            }
            else {

                let value = random(item.value[0], item.value[1])
                if (item.title == 'Probabilidade de Queda') value = value > item.value[1] ? 1 : 0
                return {
                    title: item.title,
                    value: value
                }
            }
        })
    
        const newInput : {[key : string] : any} = input.reduce((acc: {[key: string]: typeof item.value}, item) => {
            acc[item.title] = item.value;
            return acc; 
        }, {});
    

        newInput['Position'] = {
            x: newInput['Posição do Capacete (X)'],
            y: newInput['Posição do Capacete (Y)'],
            z: newInput['Posição do Capacete (Z)']
        }
    
        newInput['Gases Tóxicos'] = {
            'Monóxido de Carbono': newInput['Gases Tóxicos (Monóxido de Carbono)'],
            'Metano': newInput['Gases Tóxicos (Metano)']
        }


        const dataMQTT : DataMQTT = {
            HelmetNB: idCapacete,
            Fall: newInput['Probabilidade de Queda'] == 1,
            BodyTemperature: newInput['Temperatura Corporal'],
            Heartrate: newInput['Ritmo Cardíaco'],
            Proximity: newInput['Proximidade'],
            Location: {
                X: newInput['Posição do Capacete (X)'],
                Y: newInput['Posição do Capacete (Y)'],
                Z: newInput['Posição do Capacete (Z)']
            },
            Gases: {
                Metano: newInput['Gases Tóxicos (Metano)'],
                MonoxidoCarbono: newInput['Gases Tóxicos (Monóxido de Carbono)']
            }
        }
        
        mqtt.publish('dados', JSON.stringify(dataMQTT))
    }

}


export const useMQTTStore = defineStore('mqtt', {
    state: () => ({
        mqtt: null as MqttService | null
    }),
    actions: {
        setMqtt(mqtt: MqttService) {
            this.mqtt = mqtt
        }
    }
})



export const useTaskStore = defineStore('taskMQTT', {
    state: () => ({
        tasks: [] as Task[]
    }),
    actions: {
        addTask(inputs : Array<Input>, capacetes : Array<number> ,intervalSeconds: number, mqtt : MqttService) {
            const task : Task = { 
                inputs : [...inputs],
                capacetes : [...capacetes],
                intervalSeconds
            }
            const func = () => envio(mqtt, task)
            func()
            if (intervalSeconds == 0) return
            const intervalId = setInterval(func, intervalSeconds * 1000)
            task.intervalId = intervalId
            this.tasks.push(task)
        },

        hasTask(idCapacete : number){
            return this.tasks.some((task) => task.capacetes.includes(idCapacete))
        },
        removeTask(index: number) {
            const task = this.tasks[index]
            if (task) {
                clearInterval(task.intervalId)
                this.tasks.splice(index, 1)
            }
        }
    }
})

// src/store/index.ts
import { defineStore } from 'pinia'
import type { Input } from '@/views/SimulatorView.vue'
import { MqttService } from '@/services/mqtt'

type Status = 'Stopped' | 'Running' | 'Finished'

export class Task  {
    title: string
    inputs: Record<string, Input>
    intervalId?: NodeJS.Timeout
    intervalSeconds: number
    capacetes: Array<number>
    isEdit: boolean
    status: Status
    time: Date

    constructor(title: string, inputs: Record<string, Input>, intervalSeconds: number, capacetes: Array<number>) {
        this.title = title
        this.inputs = inputs
        this.intervalSeconds = intervalSeconds
        this.capacetes = capacetes
        this.isEdit = false
        this.status = 'Running'
        this.time = new Date()
    }

    suspend(){
        this.status = 'Stopped'
        this.time = new Date()
        clearInterval(this.intervalId)
    }

    play(mqtt : MqttService){
        const { intervalSeconds } = this
        const func = () => envio(mqtt, this)
        func()
        if(intervalSeconds != 0){
            const intervalId = setInterval(func, intervalSeconds * 1000)
            this.intervalId = intervalId
            this.status = 'Running'
        }
        else{
            this.status = 'Finished'
        }
        this.time = new Date()
    }

    edit(mqtt: MqttService, title: string, inputs: Record<string, Input>, intervalSeconds: number){
        clearInterval(this.intervalId)
        this.title = title
        this.inputs = inputs
        this.intervalSeconds = intervalSeconds
        this.isEdit = false
        if(this.status == 'Running') this.play(mqtt)
    }

    removeCapacete(idCapacete: number){
        const index = this.capacetes.indexOf(idCapacete)
        if(index != -1){
            this.capacetes.splice(index, 1)
        }
        if(this.capacetes.length == 0){
            this.suspend()
        }
    }
    

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


const random = (min: number, max: number) => {
    return Math.random() * (max - min) + min
}

const envio = (mqtt: MqttService, task: Task) => {
    const { inputs, capacetes } = task
    for (const idCapacete of capacetes) {
        const input = Object.values(inputs).map((item) => {
            if (item.tipo == 'Constante') {
                return {
                    title: item.title,
                    value: item.value[0]
                }
            } else {
                let value = random(item.value[0], item.value[1])
                if (item.title == 'Probabilidade de Queda') value = random(0,1) <= value ? 1 : 0
                return {
                    title: item.title,
                    value: value
                }
            }
        })

        const newInput: { [key: string]: any } = input.reduce(
            (acc: { [key: string]: typeof item.value }, item) => {
                acc[item.title] = item.value
                return acc
            },
            {}
        )

        newInput['Position'] = {
            x: newInput['Posição do Capacete (X)'],
            y: newInput['Posição do Capacete (Y)'],
            z: newInput['Posição do Capacete (Z)']
        }

        newInput['Gases Tóxicos'] = {
            'Monóxido de Carbono': newInput['Gases Tóxicos (Monóxido de Carbono)'],
            Metano: newInput['Gases Tóxicos (Metano)']
        }

        const dataMQTT: DataMQTT = {
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
        mqtt.publish('my/topic', JSON.stringify(dataMQTT))
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
        tasks: {} as { [idObra: string]: { [idTask: string] : Task} },
        active: "" as string
    }),
    actions: {
        setActive(idObra : string){
            this.active = idObra
            if(!this.tasks[idObra]) {
                this.tasks[idObra] = {}
                console.log("Não existem tarefas para esta obra")
            }
            else{
                console.log("Existem tarefas para esta obra")
            }
        },
        addTask(mqtt: MqttService, task: Task) {
            task.play(mqtt)
            this.tasks[this.active][Date.now()] = task
            //this.tasks.push(task)
        },
        hasTask(idCapacete: number) {
            return Object.values(this.tasks[this.active]).some((task) => task.status =="Running" && task.capacetes.includes(idCapacete))
        },
        taskByCapacete(idCapacete: number) {
            //return key of task
            return Object.keys(this.tasks[this.active]).find((key) => this.tasks[this.active][key].capacetes.includes(idCapacete))
        },
        removeTask(index: string) {
            const task = this.tasks[this.active][index]
            if (task) {
                clearInterval(task.intervalId)
                delete this.tasks[this.active][index]
            }
            else{
                console.log("Não existe tarefa com esse id", index)
            }
        },
        possibleStoppedTasks(capacetes: Array<number>) {
            return Object.values(this.tasks[this.active]).filter((task) =>{
                if(task.status == 'Running')
                    return task.capacetes.some((capacete) => capacetes.includes(capacete))
            })
        },
        stopTaskByCapacetes(capacetes: Array<number>) {
            Object.values(this.tasks[this.active]).forEach((task) => {
                if(task.status == 'Running'){
                    if (task.capacetes.some((capacete) => capacetes.includes(capacete)))
                        task.suspend()
                }
            })
        }
    }
})

<script setup lang="ts">
import { ref, computed } from 'vue'
import MapEditor from '@/components/MapEditor.vue'
import PageLayout from '@/components/Layouts/PageLayout.vue'
import ObraLayout from '@/components/Layouts/ObraLayout.vue'
import { useRoute, useRouter } from 'vue-router'
import { MqttService } from '@/services/mqtt'
import { onMounted } from 'vue'
import { useTaskStore } from '@/store'
import { useMQTTStore } from '@/store'
import TaskHistory from '@/components/TaskHistory.vue'
import TaskInput from '@/components/TaskInput.vue'
import { Task } from '@/store'
import type { Mapa } from '@/interfaces'
import { ObraService } from '@/services/http'

export interface Capacete {
    position: { x: number; y: number }
    key: number
    inputs: Array<Input>
}

export interface Input {
    title: string
    value: [number, number]
    range: [number, number]
    tipo: string
    step?: number
}


let mqtt: MqttService | null = null
const mqttStore = useMQTTStore()
const inputs = ref<Array<Input>>([])
const imageUrls: Array<string> = ['/Duplex1.svg', '/Duplex2.svg']
const taskStore = useTaskStore()
const router = useRouter()
const route = useRoute()
const title = ref('')
const page = ref(1)
const tempo = ref(1)
const selected = ref<Array<number>>([])
const capacetes = ref<Array<Capacete>>([])
const taskName = ref('Tarefa')
const addCapaceteTask = ref("")
const mapList = ref<Array<Mapa>>([])
const inputsConstante: Array<Input> = [
    {
        title: 'Temperatura Corporal',
        range: [35, 42],
        value: [36.5, 37.5],
        tipo: 'Variável'
    },
    {
        title: 'Ritmo Cardíaco',
        range: [80, 200],
        value: [80, 100],
        tipo: 'Variável'
    },
    {
        title: 'Probabilidade de Queda',
        range: [0, 1],
        value: [0.5, 0.5],
        tipo: 'Variável'
    },
    {
        title: 'Proximidade',
        range: [0, 200],
        value: [0, 0],
        tipo: 'Variável'
    },
    {
        title: 'Gases Tóxicos (Monóxido de Carbono)',
        range: [0, 1],
        value: [0, 0],
        tipo: 'Variável'
    },
    {
        title: 'Gases Tóxicos (Metano)',
        range: [0, 1],
        value: [0, 0],
        tipo: 'Variável'
    },
    {
        title: 'Posição do Capacete (X)',
        range: [0, 1],
        value: [0, 0],
        tipo: 'Variável'
    },
    {
        title: 'Posição do Capacete (Y)',
        range: [0, 1],
        value: [0, 0],
        tipo: 'Variável'
    },
    {
        title: 'Posição do Capacete (Z)',
        range: [1, imageUrls.length],
        value: [1, 1],
        step: 1,
        tipo: 'Constante'
    }
]

const getObra = () => {
    ObraService.getOneObra(route.params.id.toString()).then((answer) => {
        if(answer.mapa) mapList.value = answer.mapa
        if(answer.name) title.value = answer.name
    })
}


onMounted(() => {
    const idObra = route.params.id as string
    taskStore.setActive(idObra)
    getObra()
    if (!mqttStore.mqtt) {
        mqtt = new MqttService(undefined)
        mqttStore.setMqtt(mqtt)
    } else {
        mqtt = mqttStore.mqtt as MqttService
    }
})

const updateCapacete = (capacete: Capacete) => {
    capacetes.value = capacetes.value.map((item) => {
        if (item.key === capacete.key) {
            return capacete
        }
        return item
    })
}

const addCapacete = (capacete: Capacete) => {
    capacete.inputs = [...inputsConstante]
    capacetes.value.push(capacete)
}

const goToObraPage = () => {
    router.push('/obras/' + router.currentRoute.value.params.id)
}

const unselectCapacete = (id: number) => {
    selected.value = selected.value.filter((item) => item !== id)
    if (selected.value.length == 0) {
        if (taskEdit.value != null) {
            const index = taskEdit.value
            const task = taskStore.tasks[taskStore.active][index]
            task.isEdit = false
        }
        inputs.value = []
    }
    return
}

const selectedCapacete = (id: number) => {
    if (selected.value.includes(id)) {
        unselectCapacete(id)
        return
    }
    const hasTask = taskStore.hasTask(id)
    if (hasTask) {
        const key = taskStore.taskByCapacete(id)
        if(key){
            const task = taskStore.tasks[taskStore.active][key]
            editTask(task)
        }
    } else {
        if (taskEdit.value != null) {
            taskStore.tasks[taskStore.active][taskEdit.value].isEdit = false
        }
        selected.value.push(id)
        if (addCapaceteTask.value != "") {
            const task = taskStore.tasks[taskStore.active][addCapaceteTask.value]
            task.capacetes.push(id)
        }
        inputs.value = [...inputsConstante]
    }
}

const selectAll = () => {
    selected.value = capacetes.value.map((item) => item.key)
    inputs.value = [...inputsConstante]
}

const unselectAll = () => {
    selected.value = []
    inputs.value = []
}

const deepCopy = (obj: Array<Input>) => {
    return obj.map(a => {return {...a}})

}


const editTask = (task : Task) => {
    inputs.value = deepCopy(task.inputs)
    //clone selected array
    selected.value = [...task.capacetes]    
    taskName.value = task.title
    tempo.value = task.intervalSeconds
    if (taskEdit.value != null) {
        const last = taskEdit.value
        taskStore.tasks[taskStore.active][taskEdit.value].isEdit = false
        task.isEdit = true
        if (last == taskEdit.value) {
            task.isEdit = false
        }
    }
    else task.isEdit = true
}

const taskEdit = computed(() => {
    const editKey = Object.keys(taskStore.tasks).find(key => taskStore.tasks[key].isEdit);
    return editKey || null;
})

const changeAddCapaceteTask = (id: string) => {
    if(id == addCapaceteTask.value) {
        addCapaceteTask.value = ""
    }
    else {
        const task = taskStore.tasks[taskStore.active][id]
        selected.value = [...task.capacetes]
        addCapaceteTask.value = id
    }
}

</script>
<template>
    <page-layout>
        <ObraLayout>
            <template #map>
                <h1 class="text-center text-h3">{{ title }}</h1>
                <template v-for="(map, index) in mapList" :key="map.name">
                    <map-editor 
                        :active="index == page - 1" 
                        :edit="true" 
                        :svg="map.svg" 
                        :zones="map.zonas"
                        :capacetes-position="capacetes"
                        :capacete-selected="selected"
                        @update:zones="map.zonas = $event"
                        @addCapacete="addCapacete"
                        @selectCapacete="selectedCapacete"
                        @update::capacete="updateCapacete($event)"
                        @select-all="selectAll"
                        @unselect-all="unselectAll"
                        options="Simulador"
                    ></map-editor>
                </template>
                <v-row class="d-flex justify-center mt-5" v-if="mapList.length > 1">
                    <v-pagination v-model="page" :length="mapList.length" :total-visible="5" />
                </v-row>
            </template>
            <template #content>
                <v-row class="d-flex justify-end my-2">
                    <v-btn
                        rounded="xl"
                        size="large"
                        variant="flat"
                        color="primary"
                        @click="goToObraPage"
                    >
                        Página Obra
                    </v-btn>
                </v-row>
                <TaskInput 
                    :inputs="inputs"
                    :tempo="tempo"
                    :taskName="taskName"
                    :selected="selected"
                    @update:inputs="inputs = $event"
                    @update:tempo="tempo = Number($event)"
                    @update:taskName="taskName = $event"
                    @update:selected="selected = $event"
                />
                <TaskHistory 
                    :addCapaceteTask="addCapaceteTask"
                    @edit="editTask"
                    @addCapaceteTask="changeAddCapaceteTask"
                />
            </template>
        </ObraLayout>
    </page-layout>
</template>
<style></style>

<script setup lang="ts">
import { ref, computed } from 'vue'
import MapEditor from '@/components/MapEditor.vue'
import PageLayout from '@/components/Layouts/PageLayout.vue'
import ObraLayout from '@/components/Layouts/ObraLayout.vue'
import { useRouter } from 'vue-router'
import SimuladorInput from '@/components/SimuladorInput.vue'
import SimuladorInfo from '@/components/SimuladorInfo.vue'
import { MqttService } from '@/mqttService'
import { onMounted } from 'vue'
import { useTaskStore } from '@/store'
import { useMQTTStore } from '@/store'


let mqtt : MqttService | null = null

const mqttStore = useMQTTStore()

onMounted(() => {
    if (!mqttStore.mqtt) {
        mqtt = new MqttService(undefined)
        mqttStore.setMqtt(mqtt)
    } else {
        mqtt = mqttStore.mqtt as MqttService
    }
})


export interface Capacete {
    position : {x: number, y: number},
    key: number,
    inputs: Array<Input>
}

export interface Input {
    title: string,
    value: [number, number],
    range: [number, number]
    tipo: string
    step?: number
}
const taskStore = useTaskStore()
const router = useRouter()
const page = ref(1)
const imageUrls: Array<string> = ['/Duplex1.svg', '/Duplex2.svg']
const selected = ref<Array<number>>([])
const tipoEnvio = ref('Unidade')
const tempo = ref(1)
const capacetes = ref<Array<Capacete>>([])
const showInfo = ref(false)

const rules = [
    (v: number) => v >= 0.1|| 'Inválido (min: 0.1)',
]

const getCurrentImage = computed(() => {
    const currentIndex = page.value - 1
    const validIndex = Math.min(Math.max(currentIndex, 0), imageUrls.length - 1)
    return imageUrls[validIndex]
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
    capacete.inputs = [...inputs]
    capacetes.value.push(capacete)
}

const goToObraPage = () => {
    router.push("/obras/" + router.currentRoute.value.params.id)
}

const selectedCapacete = (id : number) => {

    if (selected.value.includes(id)) {
        selected.value = selected.value.filter((item) => item !== id)
        if(selected.value.length == 0) {
            inputSelected.value = []
        } 
    } else {
        if(selected.value.length == 0) {
            inputSelected.value = capacetes.value.find((item) => item.key === id)?.inputs || []
        }
        else{
            const capacete = capacetes.value.find((item) => item.key === id)
            if (capacete) capacete.inputs = inputSelected.value
        }
        selected.value.push(id)
    }
}

const random = (min: number, max: number) => {
    return Math.random() * (max - min) + min;
}

const applyEnvio = () => {
    let envio = {
        inputs: inputSelected.value,
        capacetes: selected.value,
        tipoEnvio: tipoEnvio.value,
        tempo: tempo.value
    }
    
    let time = tipoEnvio.value == 'Tempo' ? tempo.value : 0;
    if (mqtt)taskStore.addTask(inputSelected.value, selected.value, time, mqtt)
}

const inputSelected = ref<Array<Input>>([])


const inputs : Array<Input>  = [
    {
        title: "Temperatura Corpural",
        range: [35, 42],
        value: [36.5, 37.5],
        tipo: 'Variável'
    },
    {
        title: "Ritmo Cardíaco",
        range: [80, 200],
        value: [80, 100],
        tipo: 'Variável'
    },
    {
        title: "Probabilidade de Queda",
        range: [0, 1],
        value: [0.5, 0.5],
        tipo: 'Variável'
    },
    {
        title: "Proximidade",
        range: [0, 200],
        value: [0, 0],
        tipo: 'Variável'
    },
    {
        title: "Gases Tóxicos (Monóxido de Carbono)",
        range: [0, 1],
        value: [0, 0],
        tipo: 'Variável'
    },
    {
        title: "Gases Tóxicos (Metano)",
        range: [0, 1],
        value: [0, 0],
        tipo: 'Variável'
    },
    {
        title: "Posição do Capacete (X)",
        range: [0, 1],
        value: [0, 0],
        tipo: 'Variável'
    },
    {
        title: "Posição do Capacete (Y)",
        range: [0, 1],
        value: [0, 0],
        tipo: 'Variável'
    },
    {
        title: "Posição do Capacete (Z)",
        range: [1, imageUrls.length],
        value: [1, 1],
        step: 1,
        tipo: 'Constante'
    }
]

const editTask = (inputs : Array<Input>, capacetes : Array<number>) => {
    inputSelected.value = inputs
    selected.value = capacetes
}

</script>
<template>
    <page-layout>
        <ObraLayout>
            <template #map>
                <h1 class="text-center text-h3">Nome da Obra</h1>
                <template v-for="image in imageUrls" :key="image">
                    <map-editor :active="image === getCurrentImage" :edit="true" :svg-src="image"
                        :capacetes-position="capacetes"
                        :capacete-selected="selected"
                        @addCapacete="addCapacete"   
                        @selectCapacete="selectedCapacete"
                        @update::capacete="updateCapacete($event)"
                        options="Simulador"></map-editor>
                </template>
                <v-row class="d-flex justify-center mt-5">
                    <v-pagination v-model="page" :length="imageUrls.length" :total-visible="5" />
                </v-row>
            </template>
            <template #content>
                <v-row class="d-flex justify-end my-2">
                    <v-btn
                        rounded="xl"
                        size="large"
                        variant="flat"
                        color="primary"
                        @click="goToObraPage" >
                        Página Obra
                    </v-btn>
                </v-row>
                    <v-card height="65vh" style="overflow: auto;" rounded="xl" color="grey-lighten-5">
                        <v-card-title class=" my-4">
                            <v-row justify="space-between">
                                <v-col cols="auto" />
                                <v-col cols="auto" class="text-center text-h4">
                                    Simulador
                                </v-col>
                                <v-col cols="auto">
                                  <v-btn 
                                    v-if="!showInfo"
                                    rounded="xl"
                                    variant="flat"
                                    @click="showInfo = true"
                                    icon="mdi-information"
                                    color="info"
                                    density="compact"
                                    size="large"
                                  >
                                  </v-btn>
                                  <v-btn 
                                    v-else
                                        rounded="xl"
                                        variant="flat"
                                        @click="showInfo = false"
                                        icon="mdi-close"
                                        color="error"
                                        density="compact"
                                        size="large"
                                    >
                                    </v-btn>
                                </v-col>
                        </v-row>
                        </v-card-title>
                        <v-card-text>
                            <SimuladorInfo v-if="showInfo"/>
                            <v-row v-else>
                                <template v-if="selected.length == 0">
                                    <v-col cols="12" class="text-center"> 
                                        <p class="text-h6 mt-16">
                                            <v-icon color="info">mdi-information-outline</v-icon>
                                            Selecione ou adicione um capacete para editar os valores
                                        </p>
                                    </v-col>
                                </template>
                                <template v-else>
                                    <v-col cols="12" md="6">
                                        <v-btn 
                                            block 
                                            value="Unidade" 
                                            :color="tipoEnvio=='Unidade' ? 'primary' : 'defualt'"
                                            @click="tipoEnvio = 'Unidade'"
                                            rounded="xl"
                                        >
                                            Unidade
                                        </v-btn>
                                    </v-col>
                                    <v-col cols="12" md="6">
                                        <v-btn 
                                            block 
                                            value="Tempo" 
                                            :color="tipoEnvio=='Tempo' ? 'primary' : 'value'"
                                            @click="tipoEnvio = 'Tempo'"
                                            rounded="xl"
                                        >
                                            Período de Tempo
                                        </v-btn>
                                        <v-text-field
                                            v-if="tipoEnvio=='Tempo'"
                                            v-model="tempo"
                                            class="mt-2"
                                            :rules="rules"
                                            single-line
                                            step="0.01"
                                            type="number"
                                            variant="outlined"
                                            density="compact"
                                            label="Tempo"
                                            hint="Tempo em segundos"
                                            persistent-hint
                                        ></v-text-field>
                                    </v-col>
                                </template>
                                <template v-for="input in inputSelected" :key="input.title">
                                        <v-col cols="12" md="6">
                                            <v-card elevation="4" rounded="lg">
                                                <v-card-title>
                                                    <v-row class="mb-2">
                                                        <v-col cols="6">
                                                            <v-btn 
                                                                block
                                                                value="Constante" 
                                                                :color="input.tipo=='Constante' ? 'primary' : 'defualt'"
                                                                @click="input.tipo = 'Constante'"
                                                                rounded="lg"
                                                                >
                                                                Constante
                                                            </v-btn>
                                                        </v-col>
                                                        <v-col cols="6">
                                                            <v-btn 
                                                                block
                                                                value="Variável" 
                                                                :color="input.tipo=='Variável' ? 'primary' : 'value'"
                                                                @click="input.tipo = 'Variável'"
                                                                rounded="lg"
                                                            >
                                                                Variável
                                                            </v-btn>
                                                        </v-col>
                                                    </v-row>
                                                </v-card-title>
                                                <v-card-text>
                                                    <SimuladorInput 
                                                    @updateValue="input.value = $event"
                                                    :tipo= "input.tipo"
                                                    :title="input.title" 
                                                    :range="input.range" 
                                                    :value="input.value"
                                                    :step="input.step"
                                                    />
                                                </v-card-text>
                                            </v-card>
                                        </v-col>
                                </template>
                            </v-row>
                        </v-card-text>
                    </v-card>
                <v-card class="my-4" rounded="xl" color="grey-lighten-5">
                    <v-card-text >
                        <v-row>
                            <v-col cols="8" class="text-h6">
                                Número de Capacetes Selecionados: <strong>{{selected.length}}</strong> 
                            </v-col>
                            <v-col>
                                <v-btn 
                                    block 
                                    color="primary"
                                    rounded="xl"
                                    @click="applyEnvio"
                                    :disabled="selected.length == 0"
                                >
                                    Aplicar
                                </v-btn>
                            </v-col>
                        </v-row>
                        <v-divider class="mt-4" />
                        <v-list lines="one" v-if="taskStore.tasks.length > 0" bg-color="transparent" >
                            <v-list-group 
                                v-for="(task,index) in taskStore.tasks"
                                :key="index"
                            >
                                <template #activator="{ props }">
                                    <v-list-item
                                        :key="index"
                                        :title="'Tarefa #' + index"
                                        :subtitle="`Envio de dados a cada ${task.intervalSeconds} segundos`"
                                        v-bind="props"
                                    >
                                    <template #prepend>
                                        <v-btn
                                            class="mr-2"
                                            variant="flat"
                                            icon="mdi-close"
                                            color="error"
                                            density="compact"
                                            @click="taskStore.removeTask(index)"
                                        >
                                        </v-btn>
                                        <v-btn 
                                            class="mr-2"
                                            variant="flat"
                                            icon="mdi-pencil"
                                            color="grey"
                                            density="compact"
                                            @click="editTask(task.inputs,task.capacetes)"
                                        >
                                        </v-btn>
                                    </template>
                                </v-list-item>
                                </template>
                                <v-list-item 
                                    v-for="capacete in task.capacetes"
                                    prepend-icon="mdi-account-circle"
                                    :key="capacete"
                                    :title="'ID: ' + capacete"
                                ></v-list-item>
                            </v-list-group>
                        </v-list>
                    </v-card-text>
                </v-card>
            </template>
        </ObraLayout>
    </page-layout>
</template>
<style>
</style>

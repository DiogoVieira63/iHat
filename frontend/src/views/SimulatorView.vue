<script setup lang="ts">
import { ref, computed, watch, toRaw } from 'vue'
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
import type { Capacete } from '@/interfaces'
import LogsCapacete from '@/components/LogsCapacete.vue'

export interface Input {
    title: string
    value: [number, number]
    range: [number, number]
    tipo: string
    step?: number
}

const router = useRouter()
const route = useRoute()

const mqttStore = useMQTTStore()
const taskStore = useTaskStore()
let mqtt: MqttService | null = null

const idObra = route.params.id as string

const inputs = ref<Record<string, Input>>({})
const title = ref('')
const page = ref(1)
const tempo = ref(1)
const selected = ref<Array<number>>([])
const capacetes = ref<Array<Capacete>>([])
const taskName = ref('Tarefa')
const addCapaceteTask = ref('')
const mapList = ref<Array<Mapa>>([])
const rangeMaps = ref<Array<{ x: number; y: number }>>([])
const isLoaded = ref(false)
const logSelected = ref()

const numberMaps = computed(() => {
    return mapList.value.length
})

const isSelectingPosition = ref(false)
const inputsConstante: Record<string, Input> = {
    'Temperatura Corporal': {
        title: 'Temperatura Corporal',
        range: [34, 42],
        value: [36.5, 37.5],
        tipo: 'Variável'
    },
    'Ritmo Cardíaco': {
        title: 'Ritmo Cardíaco',
        range: [80, 200],
        value: [80, 100],
        tipo: 'Variável'
    },
    'Probabilidade de Queda': {
        title: 'Probabilidade de Queda',
        range: [0, 1],
        value: [0, 0],
        tipo: 'Variável'
    },
    Proximidade: {
        title: 'Proximidade',
        range: [0, 200],
        value: [0, 0],
        tipo: 'Variável'
    },
    'Gases Tóxicos (Monóxido de Carbono)': {
        title: 'Gases Tóxicos (Monóxido de Carbono)',
        range: [0, 1],
        value: [0, 0],
        tipo: 'Variável'
    },
    'Gases Tóxicos (Metano)': {
        title: 'Gases Tóxicos (Metano)',
        range: [0, 1],
        value: [0, 0],
        tipo: 'Variável'
    },
    'Posição do Capacete (X)': {
        title: 'Posição do Capacete (X)',
        range: [-1, 1],
        value: [0, 0],
        tipo: 'Variável'
    },
    'Posição do Capacete (Y)': {
        title: 'Posição do Capacete (Y)',
        range: [-1, 1],
        value: [0, 0],
        tipo: 'Variável'
    }
}

const resetInputs = () => {
    inputs.value = structuredClone(inputsConstante)
    inputs.value['Posição do Capacete (Z)'] = {
        title: 'Posição do Capacete (Z)',
        range: [1, numberMaps.value],
        value: [1, 1],
        step: 1,
        tipo: 'Constante'
    }
    setInputMapSize(page.value - 1)
}

const getObra = () => {
    return ObraService.getOneObra(route.params.id.toString()).then((answer) => {
        if (answer.mapa) mapList.value = answer.mapa
        if (answer.nome) title.value = answer.nome
    })
}

const getCapacetesObra = () => {
    capacetes.value = []
    return ObraService.getCapacetesFromObra(idObra).then((answer) => {
        answer.forEach((capacete) => {
            capacetes.value.push(capacete)
        })
    })
}

const setupMqtt = async () => {
    //make this async
    if (!mqttStore.mqtt) {
        mqtt = new MqttService(undefined)
        mqttStore.setMqtt(mqtt)
    } else {
        mqtt = mqttStore.mqtt as MqttService
    }
}

onMounted(async () => {
    const idObra = route.params.id as string
    taskStore.setActive(idObra)
    await Promise.all([getObra(), getCapacetesObra(), setupMqtt()])
    isLoaded.value = true
})

const updateCapacete = (capacete: Capacete) => {
    capacetes.value = capacetes.value.map((item) => {
        if (item.numero === capacete.numero) {
            return capacete
        }
        return item
    })
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
        inputs.value = {}
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
        if (key) {
            const task = taskStore.tasks[taskStore.active][key]
            editTask(task)
        }
    } else {
        resetInputs()
        if (taskEdit.value != null) {
            taskStore.tasks[taskStore.active][taskEdit.value].isEdit = false
        }
        selected.value.push(id)
        if (addCapaceteTask.value != '') {
            const task = taskStore.tasks[taskStore.active][addCapaceteTask.value]
            task.capacetes.push(id)
        }
    }
}

const selectAll = () => {
    selected.value = capacetes.value.map((item) => item.numero)
    resetInputs()
}

const unselectAll = () => {
    selected.value = []
    inputs.value = {}
}

const editTask = (task: Task) => {
    inputs.value = structuredClone(toRaw(task.inputs))
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
    } else task.isEdit = true
}

const taskEdit = computed(() => {
    const editKey = Object.keys(taskStore.tasks).find((key) => taskStore.tasks[key].isEdit)
    return editKey || null
})

const changeAddCapaceteTask = (id: string) => {
    if (id == addCapaceteTask.value) {
        addCapaceteTask.value = ''
    } else {
        const task = taskStore.tasks[taskStore.active][id]
        selected.value = [...task.capacetes]
        addCapaceteTask.value = id
    }
}

const selectPosition = (value: { [key: string]: number }) => {
    const posX = inputs.value['Posição do Capacete (X)']
    const posY = inputs.value['Posição do Capacete (Y)']
    posX.value = [value.x, value.x]
    posY.value = [value.y, value.y]
    posX.tipo = 'Constante'
    posY.tipo = 'Constante'
    const posZ = inputs.value['Posição do Capacete (Z)']
    posZ.value = [page.value, page.value]
    isSelectingPosition.value = false
}

const changeSelectPosition = () => {
    isSelectingPosition.value = !isSelectingPosition.value
}

const setMapSize = (index: number, value: { x: number; y: number }) => {
    rangeMaps.value[index] = { x: Math.ceil(value.x) - 1, y: Math.ceil(value.y) - 1 }
    if (index == page.value - 1 && !isInputsEmpty.value) {
        setInputMapSize(index)
    }
}
const setInputMapSize = (index: number) => {
    inputs.value['Posição do Capacete (X)']['range'] = [0, rangeMaps.value[index]['x']]
    inputs.value['Posição do Capacete (Y)']['range'] = [0, rangeMaps.value[index]['y']]
}

watch(page, (value) => {
    if (!isInputsEmpty.value) setInputMapSize(value - 1)
})

const isInputsEmpty = computed(() => {
    return Object.keys(inputs.value).length == 0
})

const points = computed(() => {
    if (!isInputsEmpty.value) {
        return {
            x: inputs.value['Posição do Capacete (X)'].value,
            y: inputs.value['Posição do Capacete (Y)'].value
        }
    }
    return {
        x: [0, 0],
        y: [0, 0]
    }
})

const filterMenu = ref(false)
const logsFiltered = computed(() => {
    if (!logSelected.value) return taskStore.messages[taskStore.active]
    return taskStore.messages[taskStore.active].filter((item) => {
        return item.idCapacete == logSelected.value
    })
})

</script>
<template>
    <page-layout>
        <ObraLayout>
            <template #map>
                <h1 class="text-center text-h3 mb-2">{{ title }}</h1>
                <v-skeleton-loader
                    :loading="!isLoaded"
                    type="card, image"
                >
                    <template
                        v-for="(map, index) in mapList"
                        :key="map.name"
                    >
                        <map-editor
                            :active="index == page - 1"
                            :edit="isSelectingPosition"
                            :svg="map.svg"
                            :zones="map.zonas"
                            :capacetes-position="capacetes"
                            :capacetes-selected="selected"
                            :isSelectingPosition="isSelectingPosition"
                            :pointSelected="points"
                            @update:zones="map.zonas = $event"
                            @selectCapacete="selectedCapacete"
                            @update::capacete="updateCapacete($event)"
                            @select-all="selectAll"
                            @unselect-all="unselectAll"
                            @selectPosition="selectPosition"
                            @mapSize="setMapSize(index, $event)"
                        ></map-editor>
                    </template>
                    <v-row
                        class="d-flex justify-center mt-5"
                        v-if="mapList.length > 1"
                    >
                        <v-pagination
                            v-model="page"
                            :length="mapList.length"
                            :total-visible="5"
                        />
                    </v-row>
                </v-skeleton-loader>
            </template>
            <template #content>
                <v-skeleton-loader
                    v-if="!isLoaded"
                    type="card, table"
                >
                </v-skeleton-loader>
                <div v-else>
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
                        :capacetes="capacetes"
                        :selected="selected"
                        :isAdding="addCapaceteTask != ''"
                        :isSelectingPosition="isSelectingPosition"
                        @update:inputs="inputs = $event"
                        @update:tempo="tempo = Number($event)"
                        @update:taskName="taskName = $event"
                        @update:selected="selected = $event"
                        @selectCapacete="selectedCapacete"
                        @selectAll="selectAll"
                        @unselectAll="unselectAll"
                        @selectPosition="changeSelectPosition"
                    />
                    <TaskHistory
                        :addCapaceteTask="addCapaceteTask"
                        @edit="editTask"
                        @addCapaceteTask="changeAddCapaceteTask"
                    />
                </div>
            </template>
            <template #logs> 
                <v-sheet
                    width="80%"     
                    border="md"
                    class="mx-auto my-6 rounded-xl"
                >
                    <v-row class="d-flex justify-center my-6" >
                            <v-spacer></v-spacer>
                            <h1 class="text-center text-h4 ">
                                {{ logSelected ? `Alertas do capacete ${logSelected}` : 'Alertas'   }}
                            </h1>
                            <v-spacer></v-spacer>
                            <v-menu
                                v-model="filterMenu"
                                :close-on-content-click="false"
                                location="end"
                            >
                                <template #activator="{ props }">
                                    <v-btn
                                        variant="flat"
                                        color="primary"
                                        v-bind="props"
                                        icon="mdi-filter"
                                        class="mr-8"
                                    ></v-btn>
                                </template>
                                <v-card
                                    max-width="200"
                                    class="mx-auto"
                                >
                                    <v-card-text>

                                            <v-chip-group
                                                v-model="logSelected"
                                                column
                                                color="info"
                                            >
                                                <v-chip
                                                    v-for="option in capacetes.map((capacete) => capacete.numero)"
                                                    filter
                                                    variant="outlined"
                                                    :value="option"
                                                    :key="option"
                                                >
                                                    {{ option }}
                                                </v-chip>
                                            </v-chip-group>
                                    </v-card-text>
                                </v-card>
                            </v-menu>
                        <v-col cols="12">
                            <LogsCapacete
                                v-if="taskStore.messages[taskStore.active] && taskStore.messages[taskStore.active].length > 0"
                                :logs="logsFiltered"
                                @selectCapacete="logSelected = $event"
                                >
                            </LogsCapacete>
                            <v-sheet v-else class="pa-6 my-6 d-flex justify-center rounded-xl">
                                <v-alert
                                    dense
                                    type="info"
                                    class="mx-4 rounded-pill"
                                    >
                                    {{ logSelected ? `Não existem logs para o capacete ${logSelected}` : 'Selecione um capacete'  }}
                                </v-alert>
                            </v-sheet>
                        </v-col>
                    </v-row>  
                </v-sheet>
            </template>
        </ObraLayout>
    </page-layout>
</template>
<style scoped>

</style>
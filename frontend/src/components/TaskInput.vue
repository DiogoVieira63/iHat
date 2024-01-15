<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import SimuladorInput from '@/components/SimuladorInput.vue'
import SimuladorInfo from '@/components/SimuladorInfo.vue'
import { MqttService } from '@/services/mqtt'
import { onMounted } from 'vue'
import { useTaskStore } from '@/store'
import { useMQTTStore } from '@/store'
import { Task } from '@/store'
import type { PropType } from 'vue'
import type { Capacete } from '@/interfaces'
import ConfirmationDialog from './ConfirmationDialog.vue'
let mqtt: MqttService | null = null
const mqttStore = useMQTTStore()

const props = defineProps({
    selected: {
        type: Array as PropType<Array<number>>,
        required: true
    },
    capacetes: {
        type: Array as PropType<Array<Capacete>>,
        required: true
    },
    inputs: {
        type: Object as PropType<Record<string, Input>>,
        required: true
    },
    tempo: {
        type: Number,
        required: true
    },
    taskName: {
        type: String,
        required: true
    },
    isAdding: {
        type: Boolean,
        required: true
    },
    isSelectingPosition: {
        type: Boolean,
        required: true
    }
})

const emit = defineEmits([
    'update:selected',
    'update:inputs',
    'update:tempo',
    'update:taskName',
    'selectCapacete',
    'selectAll',
    'unselectAll',
    'selectPosition'
])

onMounted(() => {
    if (!mqttStore.mqtt) {
        mqtt = new MqttService(undefined)
        mqttStore.setMqtt(mqtt)
    } else {
        mqtt = mqttStore.mqtt as MqttService
    }
})

export interface Input {
    title: string
    value: [number, number]
    range: [number, number]
    tipo: string
    step?: number
}

const taskStore = useTaskStore()
const tipoEnvio = ref('Unidade')
const minTime = 0.1
const showInfo = ref(false)
const rules = [(v: number) => v >= minTime || `Inválido (min: ${minTime})`]
const rulesTaskName = [(v: string) => v.length > 0 || 'Inválido']
const formStatus = ref(false)
const menuSelected = ref(false)

const newTask = (ConfirmationDialog: boolean) => {
    if (!ConfirmationDialog) return
    let time = tipoEnvio.value == 'Tempo' ? props.tempo : 0
    const task = new Task(props.taskName, props.inputs, time, props.selected)
    // remove all capacetes from current tasks
    if (mqtt) {
        taskStore.stopTaskByCapacetes(props.selected, mqtt)
        taskStore.addTask(mqtt, task)
    }
    if (taskEdit.value) taskStore.tasks[taskStore.active][taskEdit.value].isEdit = false
    emit('update:selected', [])
    emit('update:inputs', [])
}

const saveEditTask = (ConfirmationDialog: boolean) => {
    if (!ConfirmationDialog) return
    if (mqtt && taskEdit.value != null) {
        const idObra = taskStore.active
        const task = taskStore.tasks[idObra][taskEdit.value]
        task.edit(mqtt, props.taskName, props.inputs, props.tempo)
    }
}

const disabledApply = computed(() => {
    if (formStatus.value === false) return true
    if (props.selected.length == 0) return true
    if (tipoEnvio.value == 'Tempo') {
        if (props.taskName.length == 0) return true
        if (props.tempo < minTime) return true
    }
    return false
})

const taskEdit = computed(() => {
    if (!taskStore.active) return null
    const editKey = Object.keys(taskStore.tasks[taskStore.active]).find(
        (key) => taskStore.tasks[taskStore.active][key].isEdit
    )
    return editKey || null
})

watch(taskEdit, (newValue) => {
    if (newValue != null && taskEdit.value != null) {
        const idObra = taskStore.active
        const task = taskStore.tasks[idObra][taskEdit.value]
        if (task.intervalSeconds > 0) tipoEnvio.value = 'Tempo'
        else tipoEnvio.value = 'Unidade'
    }
})

const textPossibleStopped = computed(() => {
    return taskStore.possibleStoppedTasks(props.selected).map((item) => item.title)
})

const textChangeTask = computed(() => {
    return props.selected.filter((item) => taskStore.hasTask(item))
})

const isTempo = computed(() => {
    return tipoEnvio.value == 'Tempo'
})

const isUnidade = computed(() => {
    return tipoEnvio.value == 'Unidade'
})

watch(
    () => props.isAdding,
    (value) => {
        if (value) {
            menuSelected.value = true
        }
    }
)

const isSelected = (id: number) => {
    return props.selected.includes(id)
}

const changeSelected = (id: number) => {
    emit('selectCapacete', id)
}

const selectPosition = () => {
    emit('selectPosition')
}

const capacetesLivres = computed(() => {
    //return props.capacetes
    return props.capacetes.filter((capacete) => capacete.status == 'Livre' || capacete.status == 'Associado à Obra')
})
</script>
<template>
    <v-form
        validate-on="input"
        v-model="formStatus"
        ref="form"
    >
        <v-card
            height="fit-content"
            rounded="xl"
            color="grey-lighten-5"
        >
            <v-card
                style="overflow: auto"
                variant="text"
                height="60vh"
                rounded="xl"
                color="black"
            >
                <v-card-title class="my-4">
                    <v-row justify="space-between">
                        <v-col cols="auto" />
                        <v-col
                            cols="auto"
                            class="text-center text-h4"
                        >
                            {{ taskEdit != null ? 'Editar Tarefa' : 'Nova Tarefa' }}
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
                    <SimuladorInfo v-if="showInfo" />
                    <v-row v-else>
                        <template v-if="selected.length == 0">
                            <v-col
                                cols="12"
                                class="text-center"
                            >
                                <p class="text-h6 mt-16">
                                    <v-icon color="info">mdi-information-outline</v-icon>
                                    Selecione ou adicione um capacete para editar os valores
                                </p>
                            </v-col>
                        </template>
                        <template v-else>
                            <v-col
                                cols="12"
                                md="6"
                            >
                                <v-btn
                                    block
                                    value="Unidade"
                                    :color="isUnidade ? 'primary' : 'default'"
                                    @click="tipoEnvio = 'Unidade'"
                                    rounded="xl"
                                    :disabled="taskEdit != null"
                                >
                                    Unidade
                                </v-btn>
                            </v-col>
                            <v-col
                                cols="12"
                                md="6"
                            >
                                <v-btn
                                    block
                                    value="Tempo"
                                    :color="isTempo ? 'primary' : 'default'"
                                    @click="tipoEnvio = 'Tempo'"
                                    rounded="xl"
                                    :disabled="taskEdit != null"
                                >
                                    Período de Tempo
                                </v-btn>
                            </v-col>
                            <v-col :cols="isTempo ? 6 : 12">
                                <v-text-field
                                    :model-value="taskName"
                                    @update:model-value="emit('update:taskName', $event)"
                                    class="mt-2 text-h6"
                                    single-line
                                    type="text"
                                    variant="solo"
                                    hint="Nome da Tarefa"
                                    persistent-hint
                                    :rules="rulesTaskName"
                                ></v-text-field>
                            </v-col>
                            <v-col
                                cols="6"
                                v-if="isTempo"
                            >
                                <v-text-field
                                    v-if="isTempo"
                                    :model-value="tempo"
                                    @update:model-value="emit('update:tempo', $event)"
                                    class="mt-2"
                                    :rules="rules"
                                    single-line
                                    step="0.01"
                                    type="number"
                                    variant="outlined"
                                    label="Tempo"
                                    hint="Tempo em segundos"
                                    persistent-hint
                                ></v-text-field>
                            </v-col>
                        </template>
                        <template
                            v-for="input in props.inputs"
                            :key="input.title"
                        >
                            <v-col
                                cols="12"
                                md="6"
                            >
                                <v-card
                                    elevation="4"
                                    rounded="lg"
                                >
                                    <v-card-title>
                                        <h1 class="text-h6 text-center mb-6">{{ input.title }}</h1>
                                        <v-row class="mb-2">
                                            <v-col cols="6">
                                                <v-btn
                                                    block
                                                    value="Constante"
                                                    :color="
                                                        input.tipo == 'Constante'
                                                            ? 'primary'
                                                            : 'default'
                                                    "
                                                    @click="input.tipo = 'Constante'"
                                                    rounded="xl"
                                                >
                                                    Constante
                                                </v-btn>
                                            </v-col>
                                            <v-col cols="6">
                                                <v-btn
                                                    block
                                                    value="Variável"
                                                    :color="
                                                        input.tipo == 'Variável'
                                                            ? 'primary'
                                                            : 'value'
                                                    "
                                                    @click="input.tipo = 'Variável'"
                                                    rounded="xl"
                                                >
                                                    Variável
                                                </v-btn>
                                            </v-col>
                                        </v-row>
                                    </v-card-title>
                                    <v-card-text>
                                        <SimuladorInput
                                            @updateValue="input.value = $event"
                                            :tipo="input.tipo"
                                            :range="input.range"
                                            :value="input.value"
                                            :step="input.step"
                                        />
                                    </v-card-text>
                                </v-card>
                            </v-col>
                        </template>
                        <v-col
                            cols="6"
                            class="text-center"
                            v-if="Object.keys(props.inputs).length > 0"
                        >
                            <v-card
                                elevation="4"
                                rounded="lg"
                            >
                                <v-card-title>
                                    <h1 class="text-h6 text-center mb-6">Posição</h1>
                                </v-card-title>
                                <v-card-text>
                                    <v-btn
                                        rounded="xl"
                                        :variant="props.isSelectingPosition ? 'flat' : 'outlined'"
                                        color="error"
                                        density="compact"
                                        size="x-large"
                                        @click="selectPosition"
                                        icon="mdi-map-marker"
                                    >
                                    </v-btn>
                                    <p class="my-6 text-body-1">
                                        {{
                                            props.isSelectingPosition
                                                ? 'Selecione um ponto no mapa'
                                                : 'Alterar Posição no Mapa'
                                        }}
                                    </p>
                                </v-card-text>
                            </v-card>
                        </v-col>
                    </v-row>
                </v-card-text>
            </v-card>
            <v-row class="ma-2">
                <v-col
                    cols="6"
                    class="text-h6 text-black"
                >
                    Capacetes Selecionados:
                    <v-menu
                        v-model="menuSelected"
                        :close-on-content-click="false"
                        location="top"
                    >
                        <template v-slot:activator="{ props }">
                            <v-btn
                                color="primary"
                                v-bind="props"
                                icon
                            >
                                <p class="text-white">
                                    {{ selected.length }}
                                </p>
                            </v-btn>
                        </template>
                        <v-card>
                            <v-card-title class="text-center"> Selecionados </v-card-title>
                            <v-card-text>
                                <v-list>
                                    <v-list-item
                                        class="d-flex justify-center"
                                        v-for="{ nCapacete } in capacetesLivres"
                                        :key="nCapacete"
                                    >
                                        <v-btn
                                            color="info"
                                            class="mx-2"
                                            icon
                                            :variant="isSelected(nCapacete) ? 'flat' : 'outlined'"
                                            @click="changeSelected(nCapacete)"
                                        >
                                            {{ nCapacete }}
                                        </v-btn>
                                    </v-list-item>
                                </v-list>
                                <div class="d-flex justify-space-between">
                                    <v-btn
                                        icon
                                        color="success"
                                        @click="emit('selectAll')"
                                    >
                                        <v-icon>mdi-checkbox-multiple-marked-circle-outline</v-icon>
                                    </v-btn>
                                    <v-btn
                                        icon
                                        color="error"
                                        @click="emit('unselectAll')"
                                    >
                                        <v-icon>mdi-checkbox-multiple-blank-circle-outline</v-icon>
                                    </v-btn>
                                </div>
                            </v-card-text>
                        </v-card>
                    </v-menu>
                </v-col>
                <v-col cols="3">
                    <ConfirmationDialog
                        v-if="taskEdit != null"
                        title="Guardar Alterações na Tarefa"
                        :function="saveEditTask"
                    >
                        <template #button="{ prop }">
                            <v-btn
                                v-bind="prop"
                                color="success"
                                block
                                rounded="xl"
                                :disabled="disabledApply"
                            >
                                Salvar
                            </v-btn>
                        </template>
                        <template #message>
                            Tem a certeza que pretende guardar as alterações?
                        </template>
                    </ConfirmationDialog>
                </v-col>
                <v-col cols="3">
                    <ConfirmationDialog
                        title="Criar Nova Tarefa"
                        :function="newTask"
                    >
                        <template #button="{ prop }">
                            <v-btn
                                v-bind="prop"
                                color="success"
                                block
                                rounded="xl"
                                :disabled="disabledApply"
                            >
                                Criar
                            </v-btn>
                        </template>
                        <template #message>
                            <strong v-if="textChangeTask.length > 0"> Capacetes afetados: </strong>
                            <ul>
                                <li
                                    class="ml-5"
                                    v-for="(item, index) in textChangeTask"
                                    :key="index"
                                >
                                    {{ item }}<br />
                                </li>
                            </ul>
                            <strong v-if="textPossibleStopped.length > 0">
                                Tarefas que serão suspensas:
                            </strong>
                            <ul>
                                <li
                                    class="ml-5"
                                    v-for="(item, index) in textPossibleStopped"
                                    :key="index"
                                >
                                    {{ item }}<br />
                                </li>
                            </ul>
                            Tem a certeza que pretende criar uma nova Tarefa?
                        </template>
                    </ConfirmationDialog>
                </v-col>
            </v-row>
        </v-card>
    </v-form>
</template>
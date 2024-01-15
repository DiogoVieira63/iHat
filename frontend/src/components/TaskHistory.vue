<script setup lang="ts">
import { useTaskStore } from '@/store'
import { computed } from 'vue'
import { ref, onMounted } from 'vue'
import { Task } from '@/store'
import { useMQTTStore } from '@/store'
import { MqttService } from '@/services/mqtt'
import ConfirmationDialog from './ConfirmationDialog.vue'

const mqttStore = useMQTTStore()
let mqtt: MqttService | null = null

onMounted(() => {
    if (!mqttStore.mqtt) {
        mqtt = new MqttService(undefined)
        mqttStore.setMqtt(mqtt)
    } else {
        mqtt = mqttStore.mqtt as MqttService
    }
})

const props = defineProps({
    addCapaceteTask: {
        type: String,
        default: ''
    }
})

const emit = defineEmits(['edit', 'addCapaceteTask'])

const taskStore = useTaskStore()
const tab = ref('Ativas')

const cancelEditTask = (index: number | string) => {
    taskStore.tasks[taskStore.active][index].isEdit = false
}
const taskLength = computed(() => {
    if (!taskStore.tasks[taskStore.active]) return 0
    return Object.values(taskStore.tasks[taskStore.active]).filter(
        (task) => task.status === tab.value
    ).length
})

const taskAtivasLength = computed(() => {
    if (!taskStore.tasks[taskStore.active]) return 0
    return Object.values(taskStore.tasks[taskStore.active]).filter(
        (task) => task.status === 'Running'
    ).length
})

const taskStoppedLength = computed(() => {
    if (!taskStore.tasks[taskStore.active]) return 0
    return Object.values(taskStore.tasks[taskStore.active]).filter(
        (task) => task.status === 'Stopped'
    ).length
})

const taskFinishedLength = computed(() => {
    if (!taskStore.tasks[taskStore.active]) return 0
    return Object.values(taskStore.tasks[taskStore.active]).filter(
        (task) => task.status === 'Finished'
    ).length
})

const taskEmpty = (task: Task) => {
    return task.capacetes.length === 0
}

const formatDate = (date: Date) => {
    return date.toLocaleTimeString('pt-pt', {
        hour12: false,
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit'
    })
}

const description = (task: Task) => {
    if (task.isEdit) return 'Editar..'
    switch (task.status) {
        case 'Running':
            return `A cada ${task.intervalSeconds} seg desde ${formatDate(task.time)}`
        case 'Stopped':
            return `Suspensa em ${formatDate(task.time)}`
        case 'Finished':
            return `Terminada em ${formatDate(task.time)}`
    }
}

const iconAction = (status: string) => {
    switch (status) {
        case 'Running':
            return 'mdi-pause'
        case 'Stopped':
            return 'mdi-play'
        case 'Finished':
            return 'mdi-restart'
    }
}

const colorAction = (status: string) => {
    switch (status) {
        case 'Running':
            return 'error'
        case 'Stopped':
            return 'success'
        case 'Finished':
            return 'warning'
    }
}
const titleAction = (status: string) => {
    switch (status) {
        case 'Running':
            return 'Suspender'
        case 'Stopped':
            return 'Recomeçar'
        case 'Finished':
            return 'Recomeçar'
    }
}

const functionAction = (status: string, task: Task, ConfirmationDialog: boolean) => {
    if (!ConfirmationDialog) return
    switch (status) {
        case 'Running':
            task.suspend()
            if (mqtt) taskStore.suspendTask(mqtt, task)
            break
        case 'Stopped':
        case 'Finished':
            if (mqtt) {
                taskStore.stopTaskByCapacetes(task.capacetes, mqtt)
                task.play(mqtt)
            }
            break
    }
}

const removeCapacete = (task: Task, capacete: number, ConfirmationDialog: boolean) => {
    if (!ConfirmationDialog) return
    if (mqtt) task.removeCapacete(capacete, mqtt)
}

const removeTask = (index: string, ConfirmationDialog: boolean) => {
    if (!ConfirmationDialog) return
    if(mqtt)taskStore.removeTask(index,mqtt)
}

const tasks = computed(() => {
    if (!taskStore.tasks[taskStore.active]) return {}
    return Object.fromEntries(
        Object.entries(taskStore.tasks[taskStore.active]).filter(
            (task) => task[1].status === tab.value
        )
    )
})

const isAddCapaceteTask = computed(() => {
    return props.addCapaceteTask != ''
})
</script>
<template>
    <v-card
        class="my-4"
        rounded="xl"
        color="grey-lighten-5"
    >
        <v-card-title>
            <v-tabs
                v-model="tab"
                bg-color="grey-lighten-1"
                color="black"
                grow
                class="rounded-xl"
            >
                <v-tab value="Running"
                    >Ativas
                    <sup>{{ taskAtivasLength }}</sup>
                </v-tab>
                <v-tab value="Stopped"
                    >Suspensas
                    <sup>{{ taskStoppedLength }}</sup>
                </v-tab>
                <v-tab value="Finished"
                    >Unidade
                    <sup>{{ taskFinishedLength }} </sup>
                </v-tab>
            </v-tabs>
        </v-card-title>
        <v-card-text>
            <v-alert
                v-if="taskLength == 0"
                type="info"
                rounded="xl"
            >
                Nenhuma tarefa nesta categoria.
            </v-alert>
            <v-list
                lines="two"
                bg-color="transparent"
            >
                <v-list-group
                    v-for="(task, key) in tasks"
                    :key="key"
                >
                    <template #activator="{ props, isOpen }">
                        <v-list-item
                            :key="key"
                            v-if="task.status == tab"
                        >
                            <v-list-item-title>{{ task.title }}</v-list-item-title>
                            <v-list-item-subtitle style="line-height: normal">
                                {{ description(task) }}
                            </v-list-item-subtitle>
                            <template #append>
                                <v-btn
                                    class="mr-2"
                                    variant="text"
                                    :icon="isOpen ? 'mdi-menu-up' : 'mdi-menu-down'"
                                    density="compact"
                                    v-bind="props"
                                />

                                <ConfirmationDialog
                                    title="Remover Tarefa"
                                    :function="removeTask"
                                    :function-params="[key]"
                                >
                                    <template #button="{ prop }">
                                        <v-btn
                                            v-bind="prop"
                                            class="mr-2"
                                            variant="flat"
                                            icon="mdi-close"
                                            color="error"
                                            density="compact"
                                        />
                                    </template>
                                    <template #message>
                                        Tem a certeza que pretende remover a Tarefa?
                                    </template>
                                </ConfirmationDialog>
                            </template>
                            <template #prepend>
                                <v-btn
                                    v-if="task.isEdit"
                                    class="mr-2"
                                    variant="flat"
                                    icon="mdi-cancel"
                                    color="grey"
                                    density="compact"
                                    @click="cancelEditTask(key)"
                                />
                                <v-btn
                                    v-else
                                    class="mr-2"
                                    variant="flat"
                                    icon="mdi-pencil"
                                    color="grey"
                                    density="compact"
                                    @click="emit('edit', task)"
                                />
                                <ConfirmationDialog
                                    :title="titleAction(task.status) + ' Tarefa'"
                                    :function="functionAction"
                                    :function-params="[task.status, task]"
                                >
                                    <template #button="{ prop }">
                                        <v-btn
                                            v-bind="prop"
                                            class="mr-2"
                                            variant="flat"
                                            :icon="iconAction(task.status)"
                                            :color="colorAction(task.status)"
                                            density="compact"
                                            :disabled="taskEmpty(task)"
                                        />
                                    </template>
                                    <template #message>
                                        Tem a certeza que pretende completer a ação?
                                    </template>
                                </ConfirmationDialog>
                            </template>
                        </v-list-item>
                    </template>

                    <v-list-item
                        v-for="capacete in task.capacetes"
                        :key="capacete"
                        :title="capacete"
                    >
                        <template #prepend>
                            <img
                                src="/helmet.svg"
                                width="30px"
                                height="30px"
                                class="mr-2"
                            />
                        </template>
                        <template #append>
                            <ConfirmationDialog
                                title="Remover o Capacete"
                                :function="removeCapacete"
                                :function-params="[task, capacete]"
                            >
                                <template #button="{ prop }">
                                    <v-btn
                                        v-bind="prop"
                                        class="mr-2"
                                        variant="flat"
                                        icon="mdi-close"
                                        color="error"
                                        density="compact"
                                    />
                                </template>
                                <template #message>
                                    Tem a certeza que pretende remove o Capacete
                                    <b>{{ capacete }}</b> desta tarefa?
                                </template>
                            </ConfirmationDialog>
                        </template>
                    </v-list-item>
                    <v-list-item v-if="tab != 'Running'">
                        <v-list-item-title>
                            <v-btn
                                class="mr-2"
                                variant="flat"
                                :icon="isAddCapaceteTask ? 'mdi-close' : 'mdi-plus'"
                                density="compact"
                                :color="isAddCapaceteTask ? 'error' : 'success'"
                                @click="emit('addCapaceteTask', key)"
                            />
                            {{
                                isAddCapaceteTask
                                    ? 'Terminar Adição de Capacete'
                                    : 'Adicionar Capacete'
                            }}
                        </v-list-item-title>
                    </v-list-item>
                </v-list-group>
            </v-list>
        </v-card-text>
    </v-card>
</template>

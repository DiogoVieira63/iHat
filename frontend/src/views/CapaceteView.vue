<script setup lang="ts">
import LiveDashboards from '@/components/LiveDashboards.vue'
import PageLayout from '@/components/Layouts/PageLayout.vue'
import LiveData from '@/components/LiveData.vue'
import LogsPageCapacete from '@/components/LogsPageCapacete.vue'
import type { Log, MensagemCapacete, Capacete } from '@/interfaces'
import { CapaceteSignalRService } from '@/services/capaceteSignalR'
import { CapaceteService, ObraService } from '@/services/http'
import { onMounted, onUnmounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import CapaceteStatus from '@/components/CapaceteStatus.vue'

const route = useRoute()
const router = useRouter()
const idCapacete: string = route.params.id.toString()
const dadosCapacete = ref<Array<MensagemCapacete>>([])
const mensagemCapacete = ref<MensagemCapacete>()
const signalRService = ref<CapaceteSignalRService>(new CapaceteSignalRService(idCapacete))
const logsCapacete = ref<Array<Log>>([])
const capacete = ref<Capacete>()
const estado = ref('')
const obra = ref()
const isEditing = ref(false)


const toggleEditing = () => {
    isEditing.value = !isEditing.value
}


const updateCapaceteData = (msgCapacete: MensagemCapacete) => {
    mensagemCapacete.value = msgCapacete 
    dadosCapacete.value.push(msgCapacete)
    // dadosCapacete.value.shift();
}

const updateCapaceteLogs = (newLog: Log) => {
    // logsCapacete.value.push(newLog)
    logsCapacete.value = [newLog, ...logsCapacete.value];
}

const getDadosCapacete = () => {
    return CapaceteService.getDadosCapacete(idCapacete).then((answer) => {
        answer.forEach((dados) => {
            dadosCapacete.value.push(dados)
        })
        dadosCapacete.value = dadosCapacete.value.sort(function (a, b) {
            if (a.timestamp < b.timestamp) return -1
            else if (a.timestamp > b.timestamp) return 1
            else return 0
        })
    })
}

const sortLogsByNewest = () => {
    logsCapacete.value = logsCapacete.value.sort(function (a, b) {
            if (a.timestamp < b.timestamp) return 1
            else if (a.timestamp > b.timestamp) return -1
            else return 0
    })
}

const getLogsCapacete = () => {
    return CapaceteService.getLogsCapaceteObra(idCapacete).then((answer) => {
        answer.forEach((dados) => {
            logsCapacete.value.push(dados)
        })
        sortLogsByNewest()
    })
}


const getUltimaMensagemFromDadosCapacete = () => {
    let latestMessage = dadosCapacete.value.reduce((prev, current) =>
        prev.timestamp > current.timestamp ? prev : current
    )

    mensagemCapacete.value = latestMessage
}

const getCapacete = () =>{
    return CapaceteService.getOneCapacete(idCapacete).then((answer) => {
        console.log(answer)
        capacete.value = answer
        estado.value = capacete.value.status
        if (capacete.value.obra) {
            getObra(capacete.value.obra)
        }
    }).catch((error) => {
        throw new Error("Capacete não encontrado");
    })
}

const getObra = (idObra : string) => {
    return ObraService.getOneObra(idObra).then((answer) => {
        if (answer.nome) obra.value = answer.nome
    })
}


onMounted(async () => {
    await getCapacete().catch((error) => {
        router.push('/404')
    })
    await Promise.all([
        signalRService.value.start(),
        getDadosCapacete(),
        getLogsCapacete(),
    ])
    if (!mensagemCapacete.value && dadosCapacete.value.length > 0){
        getUltimaMensagemFromDadosCapacete()
    }
    signalRService.value.updateCapaceteData(updateCapaceteData);
    signalRService.value.updateCapaceteLogs(updateCapaceteLogs);
});

onUnmounted(() => {
    signalRService.value.close()
})


const changeEstadoCapacete = async (value: string) => {
    await CapaceteService.changeEstadoCapacete(Number(idCapacete), value)
        .then(() => {
            getCapacete()
        })
        .catch((error) => {
            console.error('Error changing state:', error)
        })
}
</script>
<template>
    <PageLayout>
        <v-row
            class="my-3 d-flex justify-center"
        >
            <v-col
                cols="12"
                md="6"
                class="d-flex justify-center"
            >
                <div
                    class="text-h4 text-lg-h3"
                >
                   Capacete {{ capacete?.numero  }}
                </div>
            </v-col>
            <v-col cols="12" md="auto">
                <CapaceteStatus 
                    v-if="isEditing" 
                    :estado="estado"
                    :idCapacete="Number(idCapacete)"
                    :canEdit="true"
                    @changeStatus="changeEstadoCapacete"
                >
                </CapaceteStatus>
                <v-alert
                    v-else
                    dense
                    class="mx-4 rounded-pill"
                    color="info"
                    width="fit-content"
                >
                    Estado do Capacete: {{ estado }}
                </v-alert>
            </v-col>
            <v-col cols="12" md="auto">
                <v-alert
                    v-if="obra && capacete"
                    style="cursor: pointer;"
                    @click="router.push(`/obras/${capacete.obra}`)"
                    dense
                    class="mx-4 rounded-pill"
                    color="info"
                    width="fit-content"
                >
                    Obra: {{ obra }}
                </v-alert>
            </v-col>
            <v-spacer />
            <v-col class="d-flex justify-end mr-6" cols="auto">
                <v-btn
                    rounded="xl"
                    :variant="isEditing ? 'outlined' : 'flat'"
                    color="primary"
                    icon="mdi-pencil"
                    @click="toggleEditing"
                    class="ml-4"
                ></v-btn>
            </v-col>
        </v-row>
        <v-row
            justify="center"
            class="ma-2"
            style="display: flex; flex-wrap: wrap;"
        >
            <v-col
                cols="12"
                lg="6"
            >
                <LiveData :emUso="estado == 'Em Uso'" :idCapacete="idCapacete" :mensagemCapacete="(mensagemCapacete as MensagemCapacete)"/>
                <LogsPageCapacete :logs="logsCapacete" />
            </v-col>
            <v-col
                cols="12"
                lg="6"
            >
                <v-card
                    class="mx-auto"
                    prepend-icon="mdi-chart-line"
                    style="height: 100%;"
                >
                    <template v-slot:title> Charts </template>
                    <v-card-text>
                        <LiveDashboards :idCapacete="idCapacete" :dadosCapacete="dadosCapacete" />
                    </v-card-text>
                </v-card>
            </v-col>
        </v-row>
    </PageLayout>
</template>

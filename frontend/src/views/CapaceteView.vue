<script setup lang="ts">
import PageLayout from '@/components/Layouts/PageLayout.vue'
import LiveData from '@/components/LiveData.vue'
import { ref, onMounted , onUnmounted } from 'vue'
import { useRoute } from 'vue-router'
import { CapaceteSignalRService } from '@/services/capaceteSignalR'
import type { MensagemCapacete, Log } from '@/interfaces';
import { CapaceteService } from '@/services/http'
import Dashboards from '@/components/Dashboards.vue'
import LogsPageCapacete from '@/components/LogsPageCapacete.vue'

const route = useRoute()
const idCapacete: string = route.params.id.toString()
const dadosCapacete = ref<Array<MensagemCapacete>>([])
const mensagemCapacete = ref<MensagemCapacete>()
const signalRService = ref<CapaceteSignalRService>(new CapaceteSignalRService(idCapacete))
const logsCapacete = ref<Array<Log>>([])

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
    );

    mensagemCapacete.value = latestMessage;
}

onMounted(async () => {
    await Promise.all([
        signalRService.value.start(),
        getDadosCapacete(),
        getLogsCapacete()
    ])
    if (!mensagemCapacete.value && dadosCapacete.value.length > 0){
        getUltimaMensagemFromDadosCapacete()
    }
    signalRService.value.updateCapaceteData(updateCapaceteData);
    signalRService.value.updateCapaceteLogs(updateCapaceteLogs);
});

onUnmounted(() => {
    signalRService.value.close();
});

</script>

<template>
    <PageLayout>
        <v-row
            justify="center"
            class="ma-2"
            style="display: flex; flex-wrap: wrap;"
        >
            <v-col
                cols="12"
                lg="6"
            >
                <LiveData :idCapacete="idCapacete" :mensagemCapacete="(mensagemCapacete as MensagemCapacete)"/>
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
                        <Dashboards :idCapacete="idCapacete" :dadosCapacete="dadosCapacete" />
                        <!-- <ExampleGraph /> -->
                    </v-card-text>
                </v-card>
            </v-col>
        </v-row>
    </PageLayout>
</template>

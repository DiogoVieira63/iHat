<script setup lang="ts">
import PageLayout from '@/components/Layouts/PageLayout.vue'
import ExampleGraph from '@/components/ExampleGraph.vue'
import LiveData from '@/components/LiveData.vue'
import { ref, onMounted , onUnmounted } from 'vue'
import { useRoute } from 'vue-router'
import { CapaceteSignalRService } from '@/services/capaceteSignalR'
import type { MensagemCapacete } from '@/interfaces';
import { CapaceteService } from '@/services/http'

const route = useRoute()
const idCapacete: string = route.params.id.toString()
const dadosCapacetes = ref<Array<MensagemCapacete>>([])
const mensagemCapacete = ref<MensagemCapacete>()
// const ultimaMensagem = ref<MensagemCapacete>()
const signalRService = ref<CapaceteSignalRService>(new CapaceteSignalRService(idCapacete))

const updateCapaceteData = (msgCapacete: MensagemCapacete) => {
    mensagemCapacete.value = msgCapacete 
}

const getDadosCapacete = () => {
    return CapaceteService.getDadosCapacete(idCapacete).then((answer) => {
        answer.forEach((dados) => {
            dadosCapacetes.value.push(dados)
        })
    })
}

const getUltimaMensagemFromDadosCapacete = () => {
    let latestMessage = dadosCapacetes.value.reduce((prev, current) =>
        prev.timestamp > current.timestamp ? prev : current
    );

    mensagemCapacete.value = latestMessage;
}

onMounted(async () => {
    await Promise.all([
        signalRService.value.start(),
        getDadosCapacete()
    ])
    if (!mensagemCapacete.value){
        getUltimaMensagemFromDadosCapacete()
        console.log("ola get ultima")
    }
    signalRService.value.updateCapaceteData(updateCapaceteData);
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
        >
            <v-col
                cols="12"
                lg="6"
            >
                <LiveData v-if="mensagemCapacete" :idCapacete="idCapacete" :mensagemCapacete="(mensagemCapacete as MensagemCapacete)"/>
            </v-col>
            <v-col
                cols="12"
                lg="6"
            >
                <v-card
                    class="mx-auto"
                    prepend-icon="mdi-chart-line"
                >
                    <template v-slot:title> Charts </template>
                    <v-card-text>
                        <ExampleGraph />
                    </v-card-text>
                </v-card>
            </v-col>
        </v-row>
    </PageLayout>
</template>

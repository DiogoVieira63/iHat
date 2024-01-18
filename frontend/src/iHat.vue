<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useNotificacoesStore } from '@/store/notifications'
import type { Obra, Log } from '@/interfaces'
import { ObraService } from './services/http'
import { ObraSignalRService } from '@/services/obraSignalR'
import { onUnmounted } from 'vue'

const obras = ref<Array<Obra>>([])
const connections = ref<Array<ObraSignalRService>>([])
const notificacoesStore = useNotificacoesStore()

const getLogsObra = (idObra: string) => {
    return ObraService.getLogsObra(idObra).then((answer) => {
        answer.forEach((log) => {
            notificacoesStore.addNotification(log)
        })
    })
}

const getObras = () => {
    obras.value = []
    return ObraService.getObras().then((answer) => {
        answer.forEach((obra) => {
            obras.value.push(obra)
        })
        obras.value = obras.value.sort(function (a, b) {
            if (a.nome < b.nome) return -1
            else if (a.nome > b.nome) return 1
            else return 0
        })
    })
}

const updateLogs = (newLog: Log) => {
    notificacoesStore.addNotification(newLog)
}

onMounted(async () => {
    if (notificacoesStore.first) {
        await getObras()
        notificacoesStore.first = false
        for (const obra of obras.value) {
            const idObra = obra.id as string
            notificacoesStore.namesObras[idObra] = obra.nome
            await getLogsObra(idObra)
            const signalRService = new ObraSignalRService(idObra)
            await signalRService.start()
            signalRService.handleIncomingLogs(updateLogs)
            connections.value.push(signalRService)
        }
    }
})

onUnmounted(() => {
    connections.value.forEach((connection) => {
        connection.close()
    })
})

</script>
<template>
    <router-view />
</template>

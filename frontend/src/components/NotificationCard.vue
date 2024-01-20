<script setup lang="ts">
import { useNotificacoesStore } from '@/store/notifications'
import type { Log } from '@/interfaces'
import { LogService } from '@/services/http'
import { useRouter } from 'vue-router'
import type { PropType } from 'vue';

const notificacoesStore = useNotificacoesStore()
const router = useRouter()


const props = defineProps({
    item: {
        type: Object as PropType<Log>,
        required: true
    }
})

const titleNotificacao = (idObra : string) => {
    return `Alerta - ${notificacoesStore.namesObras[idObra]}`
}

const seenNotificacao = async (log: Log) => {
    if (log.vista) return
    log.vista = true
    await LogService.seenLog(log.id as string)
}

const formatData = (timestamp: Date) => {
    const stringDate = timestamp.toString()
    const date = new Date(stringDate)
    const hours = date.getHours().toString().padStart(2, '0')
    const minutes = date.getMinutes().toString().padStart(2, '0')
    const seconds = date.getSeconds().toString().padStart(2, '0')

    return `${hours}:${minutes}:${seconds}`
}

const goToObraPage = (idObra : string) => {
    router.push({path: `/obras/${idObra}`})
}

</script>
<template>
    <v-card
        style="cursor: pointer;"
        @click="goToObraPage(props.item.idObra)"
        @mouseenter="seenNotificacao(props.item)"
        :title="titleNotificacao(props.item.idObra)"
        :subtitle="`Capacete ${props.item.idCapacete}`"
        :text="`${props.item.mensagem} - ${formatData(props.item.timestamp)}`"
        variant="flat"
        :color="item.vista ? 'white' : 'grey-lighten-2'"
    >
    </v-card>
</template>

<style>

</style>
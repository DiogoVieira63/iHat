<script setup lang="ts">

import { useNotificacoesStore } from '@/store/notifications'
import { useRouter } from 'vue-router'
import { computed } from 'vue'
import type { Log } from '@/interfaces'
import { LogService } from '@/services/http'

const router = useRouter()
const notificacoesStore = useNotificacoesStore()

const notificacoes = computed(() => {
    return notificacoesStore.notificacoes
})


const titleNotificacao = (idObra : string) => {
    return `Alerta - ${notificacoesStore.namesObras[idObra]}`
}

const formatData = (dataString: any) => {
    const data = new Date(dataString)
    const hora = data.getHours()
    const minuto = data.getMinutes()
    const segundo = data.getSeconds()
    return `${hora}:${minuto}:${segundo}`
}

const seenNotificacao = async (log: Log) => {
    if (log.vista) return
    log.vista = true
    console.log(log.id + ' seen!')
    await LogService.seenLog(log.id as string)
}

const goToObraPage = (idObra : string,idLog : string) => {
    router.push({path: `/obras/${idObra}`, query:{log: idLog}})
}

</script>
<template>
    <v-app-bar
        :elevation="3"
        color="primary"
        rounded
        height="80"
    >
        <v-app-bar-nav-icon @click="router.push('/')">
            <v-img
                src="/Hotpot.ico"
                alt="Image"
                width="60"
            ></v-img>
        </v-app-bar-nav-icon>
        <v-app-bar-title><b>iHat</b></v-app-bar-title>
        <template v-slot:append>
            <v-menu
                location="bottom"
                max-height="600px"
            >
            <template v-slot:activator="{ props }">
                <v-btn
                    class="text-none"
                    stacked
                    v-bind="props"
                >
                    <v-badge
                        :content="notificacoesStore.unseenNotifications().length"
                        color="error"
                    >
                        <v-icon>mdi-bell-outline</v-icon>
                    </v-badge>
                </v-btn>
            </template>
                <v-sheet width="300px" >
                    <v-alert
                        v-if="notificacoes.length == 0"
                        dense
                        type="info"
                    >
                        Nenhuma notificação para mostrar
                    </v-alert>
                    <divv-else >
                        <v-sheet
                            v-for="(item, index) in notificacoes"
                            :key="index"
                        >
                            <v-card
                                style="cursor: pointer;"
                                @click="goToObraPage(item.idObra,item.id)"
                                @mouseenter="seenNotificacao(item)"
                                :title="titleNotificacao(item.idObra)"
                                :subtitle="`Capacete ${item.idCapacete}`"
                                :text="`${item.mensagem} - ${formatData(item.timestamp)}`"
                                variant="flat"
                                :color="item.vista ? 'white' : 'grey-lighten-2'"
                            >
                            </v-card>
                            <v-divider />
                        </v-sheet>
                    </divv-else>
                </v-sheet>
            </v-menu>
        </template>
    </v-app-bar>
</template>

<style>

</style>
<script setup lang="ts">

import { useNotificacoesStore } from '@/store/notifications'
import { useRouter } from 'vue-router'
import { computed } from 'vue'
import type { Log } from '@/interfaces'
import { LogService } from '@/services/http'
import { useTheme } from 'vuetify'

const router = useRouter()
const notificacoesStore = useNotificacoesStore()
const theme = useTheme()



const titleNotificacao = (idObra : string) => {
    return `Alerta - ${notificacoesStore.namesObras[idObra]}`
}

const formatData = (timestamp: Date) => {
    const stringDate = timestamp.toString()
    const date = new Date(stringDate)
    const hours = date.getHours().toString().padStart(2, '0')
    const minutes = date.getMinutes().toString().padStart(2, '0')
    const seconds = date.getSeconds().toString().padStart(2, '0')

    return `${hours}:${minutes}:${seconds}`
}


const seenNotificacao = async (log: Log) => {
    if (log.vista) return
    log.vista = true
    await LogService.seenLog(log.id as string)
}

const goToObraPage = (idObra : string) => {
    router.push({path: `/obras/${idObra}`})
}

const notificacoes = computed(() => {
    return notificacoesStore.notificacoes.slice().reverse()
})

function toggleTheme() {
    theme.global.name.value = theme.global.current.value.dark ? 'light' : 'dark'
}


</script>
<template>
    <v-app-bar
        :elevation="3"
        color="secondary"
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
            <v-btn
                icon
                @click="toggleTheme"
            >
                <v-icon>mdi-theme-light-dark</v-icon>
            </v-btn>
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
                    <div v-else >
                        <v-sheet
                            v-for="(item, index) in notificacoes"
                            :key="index"
                        >
                            <v-card
                                style="cursor: pointer;"
                                @click="goToObraPage(item.idObra)"
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
                    </div>
                </v-sheet>
            </v-menu>
        </template>
    </v-app-bar>
</template>

<style>

</style>
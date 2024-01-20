<script setup lang="ts">
import {ref } from 'vue'
import TheAppBar from '@/components/TheAppBar.vue';
import TheFooter from '@/components/TheFooter.vue';
import type { Log } from '@/interfaces'
import { useNotificacoesStore } from '@/store/notifications'

const notifications = ref<{[id : string]: Log}>({});

const notificacoesStore = useNotificacoesStore()
const length = ref(notificacoesStore.notificacoes.length)

notificacoesStore.$subscribe((mutation,state) => {
    if (length.value < state.notificacoes.length){
        addNotification(state.notificacoes[state.notificacoes.length - 1]);
        length.value = state.notificacoes.length
    }
});

function addNotification(log : Log) {
    if(log && !first.value){
        notifications.value[log.id] = log;
        setTimeout(() => removeNotification(log.id), 3000);
    }
  }

function removeNotification(notificationId :string) {
  delete notifications.value[notificationId];
  }

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


</script>
<template>
    <v-app>
        <TheAppBar />
        <v-main>
            <div class="notificationContainer">
                <v-slide-y-transition group>
                    <template v-if="first">
                        <v-card
                            style="cursor: pointer;"
                            :title="`${notificacoesStore.notificacoes.filter((item) => !item.vista).length} Notificações por ler`"
                            :text="`Bem-vindo ao iHat!`"
                            variant="flat"
                            color="grey-lighten-2"
                        >
                        </v-card>
                    </template>
                    <template v-else>
                        <v-card
                            v-for="notification in notifications"
                            :key="notification.id"
                            style="cursor: pointer;"
                            :title="titleNotificacao(notification.idObra)"
                            :subtitle="`Capacete ${notification.idCapacete}`"
                            :text="`${notification.mensagem} - ${formatData(notification.timestamp)}`"
                            variant="flat"
                            :color="notification.vista ? 'white' : 'grey-lighten-2'"
                        >
                        </v-card>
                    </template>
                </v-slide-y-transition>
            </div>
            <slot></slot>
        </v-main>
        <TheFooter />
    </v-app>
</template>
<style scoped>
  .notificationContainer {
    position: fixed;
    top: 90px;
    right: 10px;
    display: grid;
    grid-gap: 0.5em;
    z-index: 99;
  }
</style>
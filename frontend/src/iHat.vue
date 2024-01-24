<script setup lang="ts">
import type { Obra } from '@/interfaces'
import { ObraSignalRService } from '@/services/obraSignalR'
import { useNotificacoesStore } from '@/store/notifications'
import { onMounted, onUnmounted, ref } from 'vue'
import { ObraService } from './services/http'
import type { Log } from './interfaces'
import NotificationCard from './components/NotificationCard.vue'
import { computed } from 'vue'

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


onMounted(async () => {
    if (notificacoesStore.first) {
        await getObras()
        notificacoesStore.first = false
        for (const obra of obras.value) {
            if (obra.status == 'Em Curso'){
                const idObra = obra.id as string
                await getLogsObra(idObra)
                notificacoesStore.startConnection(idObra, obra.nome)
            }
        }
    }
})

onUnmounted(() => {
    connections.value.forEach((connection) => {
        connection.close()
    })
})


const notifications = ref<{[id : string]: Log}>({});

const length = ref(notificacoesStore.notificacoes.length)
const first = ref(true)

onMounted(() => {
    setTimeout(() => {
        first.value = false
    }, 5000);
})

notificacoesStore.$subscribe((mutation,state) => {
    if (length.value < state.notificacoes.length){
        addNotification(state.notificacoes[state.notificacoes.length - 1]);
        length.value = state.notificacoes.length
    }
});

function addNotification(log : Log) {
    if(log && !first.value){
        notifications.value[log.id] = log;
        setTimeout(() => removeNotification(log.id), 2500);
    }
  }

function removeNotification(notificationId :string) {
    if (notifications.value[notificationId]) {
        setTimeout(() => delete notifications.value[notificationId],500) 
    }
}




const notificacoesPorLer = computed(()=>{
    return notificacoesStore.notificacoes.filter((item) => !item.vista).length
}) 

</script>
<template>
    {{ notificacoesStore.namesObras}}
    <div class="notificationContainer">
        <v-slide-y-transition group>
            <template v-if="first && notificacoesPorLer > 0">
                <v-card
                    style="cursor: pointer;"
                    :title="`${notificacoesPorLer} Notificações por ler`"
                    :text="`Bem-vindo ao iHat!`"
                    variant="flat"
                    color="grey-lighten-2"
                >
                </v-card>
            </template>
            <template v-else>
                <NotificationCard
                    v-for="notification in notifications"
                    :key="notification.id"
                    :item="notification"
                    @mouseenter="removeNotification(notification.id)" 
                    
                />
            </template>
        </v-slide-y-transition>
    </div>
    <router-view />
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
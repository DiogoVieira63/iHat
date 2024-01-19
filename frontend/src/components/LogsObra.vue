<script setup lang="ts">
import type { Log } from '@/interfaces'
import type { PropType } from 'vue'
import { computed } from 'vue'



const props = defineProps({
    logs: {
        type: Array as PropType<Array<Log>>,
        required: true
    },
    capaceteSelected: {
        type: Number,
        default: null
    }
})

const emit = defineEmits(['selectCapacete'])


const getIcon = (type: string) => {
    switch (type) {
        // case 'informative':
        //   return 'mdi-information-variant-circle-outline';
        case 'Alerta':
            return 'mdi-alert-outline'
        case 'Grave':
            return 'mdi-alert-outline'
        default:
            return ''
    }
}

const getColor = (type: string) => {
    // if (type==="informative") return "info" else
    if (type === 'Alerta') return 'warning'
    else if (type === 'Grave') return 'error'
}

const formatTimestamp = (timestamp: Date) => {
    const stringDate = timestamp.toString()
    const date = new Date(stringDate)
    const day = date.getDate().toString().padStart(2, '0')
    const month = (date.getMonth() + 1).toString().padStart(2, '0')
    const year = date.getFullYear()
    const hours = date.getHours().toString().padStart(2, '0')
    const minutes = date.getMinutes().toString().padStart(2, '0')
    const seconds = date.getSeconds().toString().padStart(2, '0')

    return `${day}-${month}-${year} @ ${hours}:${minutes}:${seconds}`
}

const logsFiltered = computed(() => {
    if (!props.capaceteSelected) return props.logs
    return props.logs.filter((item) => {
        return item.idCapacete == props.capaceteSelected
    })
})

// const logsFiltered = computed(()=>{
//     if (!props.capaceteSelected) return props.logs.slice(-length.value)
//     return props.logs.slice(-length.value).filter((item)=> {
//         return item.idCapacete == props.capaceteSelected
//     })
// })

// const logsConst = 1
// const length = ref(logsConst)

// const load = async ({done} : { done: (status: any) => void }) => {
//     setTimeout(() => {
//         length.value+= logsConst;
//         done('ok');
//     }, 2000);
// }
</script>

<template>
    <v-container v-if="logsFiltered.length > 0">
        <v-infinite-scroll
            height="35vh"
            width="100%"
            side="end"
            class="ma-12"
        >
            <v-timeline>
                <v-timeline-item
                    v-for="log in logsFiltered"
                    :key="log.id"
                    :dot-color="getColor(log.type)"
                    size="large"
                    :icon="getIcon(log.type)"
                    class="me-4"
                >
                    <v-card
                        style="cursor: pointer"
                        @click="emit('selectCapacete', log.idCapacete)"
                    >
                        <v-card-title :class="'bg-' + getColor(log.type)">
                            <h2 class="font-weight-light">
                                {{ log.type }}
                            </h2>
                        </v-card-title>
                        <v-card-text class="py-2">
                            <h3>{{ log.mensagem }}</h3>
                        </v-card-text>
                        <v-card-actions>
                            <v-list-item class="w-100">
                                <template v-slot:prepend>
                                    <v-avatar image="/helmet.svg"></v-avatar>
                                </template>
                                <v-list-item-title>
                                    <b>Id Capacete:</b> {{ log.idCapacete }}
                                </v-list-item-title
                                >
                                <v-list-item-subtitle>
                                    <b>Id Trabalhador:</b> {{ log.idTrabalhador }}
                                </v-list-item-subtitle
                                >
                            </v-list-item>
                        </v-card-actions>
                    </v-card>
                    <template v-slot:opposite>
                        <div
                            class="pt-1 headline font-weight-bold"
                            v-text="formatTimestamp(log.timestamp)"
                        ></div>
                    </template>
                </v-timeline-item>
            </v-timeline>
            <template #loading>
            </template>
         </v-infinite-scroll>
    </v-container>
    <v-container v-else>
        <v-sheet
            height="35vh"
            width="400px"
            class="d-flex align-center mx-auto"
            rounded="b-xl"
        >
            <v-alert
                dense
                type="info"
                class="mx-4 rounded-pill"
            >
                Não existem logs para
                {{ capaceteSelected ? `o capacete ${capaceteSelected}` : 'esta obra' }}
            </v-alert>
        </v-sheet>
    </v-container>
</template>

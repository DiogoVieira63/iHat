<script setup lang="ts">
import type { PropType } from 'vue';

const props = defineProps({
    logs: {
        type: Array as PropType<Array<{ message: string; time: Date; idCapacete: number }>>,
        required: true
    },
    capaceteSelected: {
        type: Number,
        default: null
    }
})

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

const emit = defineEmits(['selectCapacete'])
</script>
<template>
    <v-container v-if="props.logs.length > 0">
        <v-infinite-scroll
            height="35vh"
            side="end"
        >
            <v-timeline align="start">
                <v-timeline-item
                    v-for="(log, index) in props.logs"
                    :key="index"
                    size="large"
                    class="me-4"
                    icon="mdi-alert-outline"
                    dot-color="error"
                >
                    <v-card
                        style="cursor: pointer"
                        @click="emit('selectCapacete', log.idCapacete)"
                    >
                        <v-card-title class="bg-error">
                            <h2 class="font-weight-light">Alerta</h2>
                        </v-card-title>
                        <v-card-text class="py-2">
                            <v-list-item class="w-100">
                                <template v-slot:prepend>
                                    <v-avatar image="/helmet.svg"></v-avatar>
                                </template>
                                <v-list-item-title>
                                    <b>Id Capacete:</b>
                                    {{ log.idCapacete }}</v-list-item-title
                                >
                            </v-list-item>
                        </v-card-text>
                    </v-card>
                    <template v-slot:opposite>
                        <div
                            class="pt-1 headline font-weight-bold"
                            v-text="formatTimestamp(log.time)"
                        ></div>
                    </template>
                </v-timeline-item>
            </v-timeline>
            <template #loading> </template>
        </v-infinite-scroll>
    </v-container>
    <v-container v-else>
        <v-sheet
            height="35vh"
            width="400px"
            class="d-flex align-center mx-auto"
            rounded="xl"
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

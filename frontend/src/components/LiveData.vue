<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

const routeId = ref('')
const liveData = ref([
    { type: 'heart-rate', value: 80, severity: 'normal' },
    { type: 'temperature', value: 37.0, severity: 'normal' },
    { type: 'gases', value: 'Moderate', severity: 'slightly_bad' },
    { type: 'fall-detection', value: 'Yes', severity: 'bad' }
])

onMounted(() => {
    if (typeof route.params.id === 'string') {
        routeId.value = route.params.id
    }
})

const getIcon = (type: string) => {
    switch (type) {
        case 'heart-rate':
            return 'mdi-heart-pulse'
        case 'temperature':
            return 'mdi-thermometer'
        case 'gases':
            return 'mdi-cloud-alert'
        case 'fall-detection':
            return 'mdi-ambulance'
        default:
            return ''
    }
}

const getTitle = (type: string) => {
    switch (type) {
        case 'heart-rate':
            return 'Frequência Cardíaca'
        case 'temperature':
            return 'Temperatura Corporal'
        case 'gases':
            return 'Gases Tóxicos'
        case 'fall-detection':
            return 'Indicador de Queda'
        default:
            return ''
    }
}

const getColor = (severity: string) => {
    //var red = '#fd5050'
    var red = '#ff9999'
    var yellow = '#fcff99'
    var green = '#a6ffbe'

    if (severity === 'bad') return red
    else if (severity === 'slightly_bad') return yellow
    else if (severity === 'normal') return green
}
</script>
<template>
    <v-card class="mx-auto" color="#f6f6f6" prepend-icon="mdi-hard-hat" height="auto">
        <template v-slot:title>
            <h2>Capacete {{ routeId }} - Live Data</h2>
        </template>

        <v-row class="px-16 py-8">
            <v-col v-for="(item, index) in liveData" :key="index" cols="12" md="6">
                <!-- Ajeitar caso sejam precisos mais dados -->
                <v-card
                    class="mx-4 my-6"
                    :color="getColor(item.severity)"
                    :prepend-icon="getIcon(item.type)"
                    height="250px"
                >
                    <template v-slot:title>
                        <h3>{{ getTitle(item.type) }}</h3>
                    </template>
                    <v-card-text class="text-center">
                        <div v-if="item.type === 'temperature'" class="text-h3">
                            <b>{{ item.value }}&deg;C</b>
                        </div>
                        <div v-else class="text-h3">
                            <b>{{ item.value }}</b>
                        </div>
                    </v-card-text>
                </v-card>
            </v-col>
        </v-row>
    </v-card>
</template>

<style scoped>
.text-center {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    height: 60%;
}
</style>

<script setup lang="ts">
import type { PropType } from 'vue'
import { useDisplay } from 'vuetify'
import type { MensagemCapacete, Gases, ValueObject } from '@/interfaces'

const { mdAndDown, xs } = useDisplay()

const props = defineProps({
    idCapacete: {
        type: String,
        required: true
    },
    mensagemCapacete: {
        type: Object as PropType<MensagemCapacete>,
        required: true
    }
})

const filteredMensagemCapacete = () => {
    if (props.mensagemCapacete) {
        const { fall, bodyTemperature, heartrate, gases } = props.mensagemCapacete

        const filteredData = {
            fall,
            bodyTemperature,
            heartrate,
            gases
        }
        return filteredData
    }
}

const getIcon = (type: string) => {
    switch (type) {
        case 'heartrate':
            return 'mdi-heart-pulse'
        case 'bodyTemperature':
            return 'mdi-thermometer'
        case 'gases':
            return 'mdi-cloud-alert'
        case 'fall':
            return 'mdi-ambulance'
        default:
            return ''
    }
}

const getTitle = (type: string) => {
    switch (type) {
        case 'heartrate':
            return 'Frequência Cardíaca'
        case 'bodyTemperature':
            return 'Temperatura Corporal'
        case 'gases':
            return 'Gases Tóxicos'
        case 'fall':
            return 'Indicador de Queda'
        default:
            return ''
    }
}

// const getColor = (severity: string) => {
//     //var red = '#fd5050'
//     var red = '#ff9999'
//     var yellow = '#fcff99'
//     var green = '#a6ffbe'

//     if (severity === 'bad') return red
//     else if (severity === 'slightly_bad') return yellow
//     else if (severity === 'normal') return green
// }

//TEMPORARIO, DEPOIS POSSIVELMENTE VITR ANÁLISE FEITA DO BACK NO OBJETO.
//ALGO DESTE TIPO:
//
const getColor = (key: string, value: boolean | ValueObject | Gases) => {
    var red = '#ff9999'
    var green = '#a6ffbe'

    if (key === 'fall') {
        if (value) return red
        else return green
    } else if (key === 'heartrate') {
        if (
            typeof value === 'object' &&
            'value' in value &&
            (value.value < 60 || value.value > 180)
        )
            return red
        else return green
    } else if (key === 'bodyTemperature') {
        if (
            typeof value === 'object' &&
            'value' in value &&
            (value.value < 34.5 || value.value > 37.4)
        )
            return red
        else return green
    } else if (key === 'gases') {
        if (
            typeof value === 'object' &&
            'metano' in value &&
            'monoxidoCarbono' in value &&
            (value.metano > 0 || value.monoxidoCarbono > 0)
        )
            return red
        else return green
    }
}
</script>
<template>
    <v-card
        class="mx-auto"
        color="#f6f6f6"
        prepend-icon="mdi-hard-hat"
        height="auto"
    >
        <template v-slot:title>
            <h3 v-if="mdAndDown">Capacete {{ props.idCapacete }} - Live Data</h3>
            <h2 v-else>Capacete {{ props.idCapacete }} - Live Data</h2>
        </template>

        <v-row class="px-4 py-4">
            <v-col
                v-for="(value, key, index) in filteredMensagemCapacete()"
                :key="index"
                cols="12"
                md="6"
                align="center"
            >
                <!-- Ajeitar caso sejam precisos mais dados -->
                <v-card
                    :color="getColor(key, value)"
                    :prepend-icon="getIcon(key)"
                    height="250px"
                    :max-width="mdAndDown ? '750px' : '30vw'"
                >
                    <template v-slot:title>
                        <h5
                            v-if="mdAndDown"
                            style="text-align: left"
                        >
                            {{ getTitle(key) }}
                        </h5>
                        <h4
                            v-else
                            style="text-align: left"
                        >
                            {{ getTitle(key) }}
                        </h4>
                    </template>
                    <v-card-text class="text-center">
                        <div v-if="key === 'fall'">
                            <v-chip class="custom-chip-size">
                                <b v-if="value == true"> Queda Detetada</b>
                                <b v-else> - </b>
                            </v-chip>
                        </div>
                        <div
                            v-else-if="key === 'gases'"
                            v-for="(dictValue, dictKey) in value"
                            :key="dictKey"
                        >
                            <v-chip
                                v-if="dictKey === 'metano'"
                                class="custom-chip-size my-4"
                            >
                                <b>CH₄: {{ dictValue }}</b>
                            </v-chip>
                            <v-chip
                                v-if="dictKey === 'monoxidoCarbono'"
                                class="custom-chip-size"
                            >
                                <b>CO : {{ dictValue }}</b>
                            </v-chip>
                        </div>
                        <div
                            v-else
                            v-for="(dictValue, dictKey) in value"
                            class="text-h3"
                        >
                            <v-chip class="custom-chip-size">
                                <b v-if="key === 'bodyTemperature'">{{ dictValue }}&deg;C</b>
                                <b v-else-if="key === 'heartrate'">{{ dictValue }} bpm</b>
                                <b v-else>{{ dictValue }}</b>
                            </v-chip>
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
    height: 65%;
}
.custom-chip-size {
    font-size: 25px !important;
    min-width: 125px !important;
    width: 15vw;
    height: 75px !important;
    justify-content: center;
    align-items: center;
}
</style>

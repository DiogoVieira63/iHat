<script setup lang="ts">
import type { Gases, MensagemCapacete, ValueObject } from '@/interfaces';
import type { PropType } from 'vue';
import { useDisplay } from 'vuetify';

const { mdAndDown } = useDisplay()

const props = defineProps({
    idCapacete: {
        type: String,
        required: true
    },
    mensagemCapacete: {
        type: Object as PropType<MensagemCapacete>,
        required: true,
    },
    emUso: {
        type: Boolean,
        required: true,
    }
})

const filteredMensagemCapacete = () => {
    if (props.mensagemCapacete){
        const { fall, bodyTemperature, heartrate, gases } = props.mensagemCapacete;

        const filteredData = {
            fall,
            bodyTemperature,
            heartrate,
            gases
        };
        return filteredData;
    }
    else {
        return {
            "fall": null,
            "bodyTemperature": {"value": null},
            "heartrate": {"value": null},
            "gases": {"metano": null, "monoxidoCarbono": null}
        } 
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

const getColor = (key: string, value: boolean | ValueObject | Gases ) => {
    var red = '#ff9999'
    var green = '#a6ffbe'
    var grey = '#f6f6f6'

    if  (value === null || 
        (typeof value === 'object' && 'value' in value && value.value === null) || 
        (typeof value === 'object' && 'metano' in value && 'monoxidoCarbono' in value && (value.metano === null || value.monoxidoCarbono === null)) ) return grey
    if (key === 'fall'){
        if (value) return red
        else return green
    }
    else if (key === 'heartrate'){
        if (typeof value === 'object' && 'value' in value && (value.value<60 || value.value>180)) return red
        else return green
    }
    else if (key === 'bodyTemperature'){
        if (typeof value === 'object' && 'value' in value && (value.value<34.5 || value.value>37.4)) return red
        else return green
    }
    else if (key === 'gases'){
        if (typeof value === 'object' && 'metano' in value && 'monoxidoCarbono' in value && (value.metano > 0 || value.monoxidoCarbono>0)) return red
        else return green
    }
}

</script>
<template>
    <v-card
        class="mx-auto"
        prepend-icon="mdi-hard-hat"
        height="auto"
    >
        <template v-slot:title>
            <h3 :class="mdAndDown ? 'text-h5' : 'text-h4' " >Live Data</h3>
        </template>
        <v-sheet
            v-if="!emUso"
            class="d-flex align-center mx-auto"
            rounded="b-xl"
            width="400px"
            height="35vh"
        >
        <v-alert
            dense
            type="info"
            class="mx-4 rounded-pill"
        >
            Capacete não está em uso
        </v-alert>
    </v-sheet>

        <v-row class="px-4 py-4" v-else>
            <v-col
                v-for="(value, key, index) in filteredMensagemCapacete()"
                :key="index"
                cols="12"
                md="6"
                align="center"
            >
                <!-- Ajeitar caso sejam precisos mais dados -->
                <v-card
                    :color="getColor(key, value as boolean | ValueObject | Gases)"
                    :prepend-icon="getIcon(key)"
                    height="250px"
                    :max-width="mdAndDown ? '750px' : '30vw'"
                >
                    <template v-slot:title>
                        <h4 :class="mdAndDown? 'text-h6': 'text-h5'" style="text-align: left;">{{ getTitle(key) }}</h4>
                    </template>
                    <v-card-text class="text-center">
                        <div
                            v-if="key === 'fall'"
                        >
                            <v-chip class="custom-chip-size" >
                                <b v-if="value == true"> Queda Detetada</b>
                                <b v-else> - </b>
                            </v-chip>
                        </div>
                        <div
                            v-else-if="key === 'gases'"
                            v-for="(dictValue, dictKey) in value"
                            :key="dictKey"
                        >
                            <v-chip v-if="dictKey==='metano'" class="custom-chip-size my-4">
                                <b v-if="dictValue !== null">CH₄: {{ dictValue }}</b>
                                <b v-else>CH₄: - </b>
                            </v-chip>
                            <v-chip v-if="dictKey==='monoxidoCarbono'" class="custom-chip-size">
                                <b v-if="dictValue !== null" >CO  : {{ dictValue }}</b>
                                <b v-else >CO  : -</b>
                            </v-chip>
                        </div>
                        <div
                            v-else
                        >
                            <div
                                v-for="(dictValue, dictKey) in value"
                                :key="dictKey"
                                class="text-h3"
                            >
                                <v-chip class="custom-chip-size">
                                    <b v-if="key==='bodyTemperature' && dictValue !== null">{{ dictValue }}&deg;C</b>
                                    <b v-else-if="key==='heartrate' && dictValue !== null">{{ dictValue }} bpm</b>
                                    <b v-else-if="dictValue === null"> - </b> 
                                    <b v-else>{{ dictValue }}</b> 
                                </v-chip>
                            </div>
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
    width: fit-content; 
    height: 75px !important;
    justify-content: center;
    align-items: center;
}

</style>
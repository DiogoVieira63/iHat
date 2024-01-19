<script setup lang="ts">
import ChartLayout from '@/components/Layouts/ChartLayout.vue'
import { ref , onMounted, watch} from 'vue'
import  type { PropType } from 'vue'
import type { MensagemCapacete } from '@/interfaces';
import type {Gases} from '@/interfaces';
/*
bodyTemperature : {value: 38}
fall : false
gases : {metano: 0, monoxidoCarbono: 0}
heartrate : {value: 100}
id : "65a557343974f29f1d390833"
location : {x: 0, y: 0, z: 0}
nCapacete : 1
position : "?"
proximity : "10"
timestamp : "2024-01-15T16:03:00.11Z"
type :  "ValueUpdate"
*/

const props = defineProps({
    idCapacete: {
        type: String,
        required: true
    },
    dadosCapacete: {
        type: Array as PropType<Array<MensagemCapacete>>,
        required: true,
    }
})

const heartrate = ref<Array<number>>([])
const bodyTemperature = ref<Array<number>>([])
const fall = ref<Array<number>>([])
const gases = ref<Array<Gases>>([])
const timestamps = ref<Array<Date>>([])

const updateChartData = () => {
    heartrate.value = []
    bodyTemperature.value = []
    fall.value = []
    timestamps.value = []
    gases.value = []
    props.dadosCapacete.forEach((data) => {
        heartrate.value.push(data.heartrate.value);
        bodyTemperature.value.push(data.bodyTemperature.value)
        fall.value.push(data.fall ? 1 : 0);
        gases.value.push(data.gases)
        timestamps.value.push(data.timestamp);
    });
};
    
onMounted(() => {
    updateChartData()
})

watch(props.dadosCapacete, (newDadosCapacete, oldDadosCapacete) => {
    updateChartData();
});

const formatTimestamp = (timestamp: Date) => {
    const stringDate = timestamp.toString()
    const date = new Date(stringDate)
    const hours = date.getHours().toString().padStart(2, '0')
    const minutes = date.getMinutes().toString().padStart(2, '0')
    const seconds = date.getSeconds().toString().padStart(2, '0')

    return `${hours}:${minutes}:${seconds}`
}

const options = (id: string) => {
    let options = {
        chart: {
            id: id
        },
        xaxis: {
            categories: timestamps.value.map(timestamp => formatTimestamp(timestamp)),
            labels: {
                rotate: -85
            },
            tickAmount: 5,
        },
    }

    if (id === "Quedas"){
        const yaxisOptions = {
            labels: {
                formatter: (value: number) => {
                    return value === 1 ? 'Queda detetada' : 'Situação normal';
                }
            },
            tickAmount: 1
        }
        options = { ...options, yaxis: yaxisOptions };
    }
    return options
}

const createSeries = (name:Array<string>, data: Array<Array<any>>) => {
    if (name.length == data.length){
        const ret = []
        for (let i = 0; i < name.length; i++) {
            ret.push({
                name: name[i],
                data: data[i]
            })
        }
        return ret
    }
}

const charts = ref([
    { name: "Frequência Cardíaca (Last 20)", type: "line", data: heartrate },
    { name: "Variação da Temperatura Corporal (Last 20)", type: "line", data: bodyTemperature },
    { name: "Gases", type: "line", data: gases },
    { name: "Quedas", type: "bar", data: fall }
]);

</script>

<template>
    <ChartLayout>
        <template 
            v-for="(chartInfo, index) in charts" 
            #[chartInfo.name]
            :key="index"
        >
            <apexchart v-if="chartInfo.name !== 'Gases'"
                :type="chartInfo.type"
                :options="options(chartInfo.name)"
                :series="createSeries([chartInfo.name], [chartInfo.data])"
            ></apexchart>
            <apexchart v-else
                :type="chartInfo.type"
                :options="options(chartInfo.name)"
                :series="createSeries(['Metano', 'Monóxido de Carbono'], [
                  chartInfo.data.map(entry => (typeof entry === 'object' ? entry.metano : 0)),
                  chartInfo.data.map(entry => (typeof entry === 'object' ? entry.monoxidoCarbono : 0))
                ])"
            ></apexchart>
        </template>
    </ChartLayout>
</template>


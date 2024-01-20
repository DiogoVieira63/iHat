<script setup lang="ts">
import ChartLayout from '@/components/Layouts/ChartLayout.vue';
import { ref } from 'vue';

const options = (id: number) => {
    const options = {
        chart: {
            id: id
        },
        xaxis: {
            categories: [1991, 1992, 1993, 1994, 1995, 1996, 1997, 1998]
        },
        stroke: {
            curve: 'smooth'
        }
    }
    return options
}

const series = [
    {
        name: 'series-1',
        data: [30, 40, 45, 50, 49, 60, 70, 91]
    }
]
const types = ref(['line', 'area', 'bar', 'heatmap', 'line', 'area', 'bar', 'heatmap']) //,"area","bar","pie","donut","radialBar","scatter","bubble","heatmap","candlestick","boxPlot","radar","polarArea","rangeBar","rangeArea","treemap"]
const slotName = (index: number) => {
    return types.value[index] + index
}
</script>

<template>
    <ChartLayout>
        <template
            v-for="(type, index) in types"
            #[slotName(index)]
            :key="index"
        >
            <apexchart
                :type="type"
                :options="options(index)"
                :series="series"
            ></apexchart>
        </template>
    </ChartLayout>
</template>

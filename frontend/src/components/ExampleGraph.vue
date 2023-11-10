<script setup>
import ChartLayout from "@/components/ChartLayout.vue";


const options = {
    chart: {
        id: 'vuechart-example'
    },
    xaxis: {
        categories: [1991, 1992, 1993, 1994, 1995, 1996, 1997, 1998]
    },
    stroke: {
        curve: 'smooth'
    },
}
const series = [{
    name: 'series-1',
    data: [30, 40, 45, 50, 49, 60, 70, 91]
}]
const types =["line","area","bar","pie","donut","radialBar","scatter","bubble","heatmap","candlestick","boxPlot","radar","polarArea","rangeBar","rangeArea","treemap"]
const slotName = (index) => {
    return "item" + (index)
}

</script>

<template>
    <ChartLayout>
        <template v-for="(type,index) in types" v-slot:[slotName(index)] >
            <apexchart :type="type" :options="options" :series="series"></apexchart>
        </template>
    </ChartLayout>
</template>

